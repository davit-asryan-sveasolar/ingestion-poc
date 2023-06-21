using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Inverters.Ingestion.Huawei.Jobs.Live.Events;
using Inverters.Ingestion.Huawei.Jobs.Live.Models;
using Inverters.Ingestion.Huawei.Jobs.Live.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vendors.Huawei.Client;
using Vendors.Huawei.Client.Models;

namespace Inverters.Ingestion.Huawei.Jobs.Live.Hosting;

public class MainService : BackgroundService
{
    private readonly HuaweiApiClient _apiClient;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly EventPublisher _eventPublisher;
    private readonly TotalToClockAlignedConverter _toClockAlignedConverter;
    private readonly HuaweiSiteDataRepository _dataRepository;
    private readonly ILogger<MainService> _logger;
    private int _plantSuccessCount;
    private int _plantFailCount;

    public MainService(HuaweiApiClient apiClient, IHostApplicationLifetime lifetime, EventPublisher eventPublisher, TotalToClockAlignedConverter toClockAlignedConverter, HuaweiSiteDataRepository dataRepository, ILogger<MainService> logger)
    {
        _apiClient = apiClient;
        _lifetime = lifetime;
        _eventPublisher = eventPublisher;
        _toClockAlignedConverter = toClockAlignedConverter;
        _dataRepository = dataRepository;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            if (!await _apiClient.LoginAsync())
            {
                _logger.LogError("Failed to login to Huawei API");
                return;
            }

            var processPlantsBlock = new TransformManyBlock<Plant[], PlantData>(ProcessPlantAsync,
                new ExecutionDataflowBlockOptions()
                {
                    EnsureOrdered = false,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                });

            var bufferPlantsBlock = new BatchBlock<Plant>(100, new GroupingDataflowBlockOptions()
            {
                EnsureOrdered = false
            });
            
            var bufferPlantDataBlock = new BatchBlock<PlantData>(10, new GroupingDataflowBlockOptions()
            {
                EnsureOrdered = false
            });

            var publishPlantMetricsBlock = new ActionBlock<PlantData[]>(PublishPlantMetricsAsync,
                new ExecutionDataflowBlockOptions()
                {
                    EnsureOrdered = false,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                });

            bufferPlantsBlock.LinkTo(processPlantsBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            processPlantsBlock.LinkTo(bufferPlantDataBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });
            bufferPlantDataBlock.LinkTo(publishPlantMetricsBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            try
            {
                var fromApi = false;
                if (fromApi)
                {
                    var allPlants = _apiClient.GetAllPlantsAsync(100);

                    await foreach (var plantPage in allPlants.WithCancellation(stoppingToken))
                    {
                        foreach (var plant in plantPage)
                        {
                            bufferPlantsBlock.Post(plant);
                        }
                    }
                }
                else
                {
                    var plants = JsonSerializer.Deserialize<Plant[]>(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "plantList.json"), stoppingToken))!;
                    foreach (var plant in plants)
                    {
                        bufferPlantsBlock.Post(plant);
                    }
                }
                
                _logger.LogInformation("Posted all plants for processing");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all plants");
            }
            finally
            {
                bufferPlantsBlock.Complete();
            }

            await publishPlantMetricsBlock.Completion;

            await _dataRepository.FlushAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed with unknown reason");
        }
        finally
        {
            _logger.LogInformation("Finished ingestion. Total Plants = {Total}, Failed = {FailedCount}. Duration={Duration}", _plantSuccessCount+_plantFailCount, _plantFailCount, sw.Elapsed);
            _lifetime.StopApplication();
            sw.Stop();
        }
    }

    private async Task PublishPlantMetricsAsync(PlantData[] plantDataBatch)
    {
        try
        {
            var points = plantDataBatch.Select(x =>
            {
                _dataRepository.Update(new HuaweiSiteData(x.RealTimeData.PlantCode,
                    x.RealTimeData.DataItemMap.TotalEnergy, x.Timestamp));
                var clockAlignedData = _toClockAlignedConverter.GetClockAligned(x.RealTimeData.PlantCode,
                    x.RealTimeData.DataItemMap.TotalEnergy, x.Timestamp);
                return clockAlignedData == null
                    ? null
                    : new PlantProductionDataPoint(x.RealTimeData.PlantCode, clockAlignedData.Value, GetLastFiveMinuteEnd(x.Timestamp));
            })
            .Where(x => x != null);

            await _eventPublisher.PublishBatchEvents(points);
            
            _logger.LogInformation("Published {BatchCount} events", plantDataBatch.Length);
            
            Interlocked.Add(ref _plantSuccessCount, plantDataBatch.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {BatchCount} events", plantDataBatch.Length);
            Interlocked.Add(ref _plantFailCount, plantDataBatch.Length);
        }
    }

    private async Task<IEnumerable<PlantData>> ProcessPlantAsync(Plant[] plants)
    {
        var plantCodes = plants.Select(x => x.PlantCode).ToArray();
        var timestamp = DateTime.UtcNow;
        _logger.LogInformation("Getting data for {PlantCount} plants for timestamp {Timestamp}", plants.Length, timestamp);
        var response = await _apiClient.GetRealTimeKpi(plantCodes);
        return response.Select(x => new PlantData(x, timestamp));
    }
    
    private DateTime GetLastFiveMinuteEnd(DateTime ts)
    {
        var minute = ts.Minute;
        if (minute % 5 != 0)
        {
            var factor = minute / 5;
            minute = factor * 5;
        }

        return new DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, minute, 0, 0, ts.Kind);
    }
}