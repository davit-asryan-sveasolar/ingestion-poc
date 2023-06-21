using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace IngestionPOC_Lambda;

public class Function
{
    private readonly InfluxDBClient _client;
    private readonly InfluxDbConfiguration _config;

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        _config = new InfluxDbConfiguration
        {
            Database = "ingestion_poc",
            Endpoint = "https://goldiewilson-538982ec.influxcloud.net:8086/",
            Username = "sveasolar_write",
            Password = @"F;>N{RgbP2%$TC4vD@8pc&;z;2gS5\"
        };
        _client = InfluxDBClientFactory.CreateV1(_config.Endpoint, _config.Username,
            _config.Password.ToCharArray(), _config.Database, "autogen");
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processing a batch of {evnt.Records.Count} records.");
        var events = evnt.Records
            .Select(x =>
                x.Body.Contains("\"TopicArn\" :") ? JsonSerializer.Deserialize<SnsPayload>(x.Body)!.Message : x.Body)
            .Select(x =>
            {
                context.Logger.LogInformation($"Record raw data: {x}.");
                return JsonSerializer.Deserialize<PlantProductionDataPoint>(x);
            }).ToArray();
        
        var influxPoints = events
            .Where(x => x != null && !string.IsNullOrEmpty(x.PlantId))
            .Select(x => PointData
                .Measurement($"production_plant_current")
                .Field("value", x.Value)
                .Tag("siteId", x.PlantId)
                .Timestamp(x.Timestamp, WritePrecision.Ns))
            .ToArray();
        
        context.Logger.LogInformation($"{influxPoints.Length} influx points collected. Writing to Influx.");
        
        await _client.GetWriteApiAsync().WritePointsAsync(influxPoints);
        
        context.Logger.LogInformation($"Finished processing a batch of {evnt.Records.Count} records.");
    }
}