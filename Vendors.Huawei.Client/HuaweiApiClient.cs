using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Vendors.Huawei.Client.Models;

namespace Vendors.Huawei.Client;

public class HuaweiApiClient
{
    private readonly ILogger<HuaweiApiClient> _logger;
    private readonly HttpClient _httpClient;

    public HuaweiApiClient(IHttpClientFactory httpClientFactory, ILogger<HuaweiApiClient> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("huawei");
    }
    
    public async Task<bool> LoginAsync()
    {
        
        var response = await _httpClient.PostAsJsonAsync("/rest/openapi/pvms/v1/login", new LoginRequest("Svea_API", "8JBYxbuoyy"), CancellationToken.None);
        if (response.Headers.TryGetValues("xsrf-token", out var values))
        {
            var value = values.FirstOrDefault();
            if (!string.IsNullOrEmpty(value))
            {
                _httpClient.DefaultRequestHeaders.Add("xsrf-token", value);
                return true;
            }
        }

        return false;
    }
    
    public async IAsyncEnumerable<IEnumerable<Plant>> GetAllPlantsAsync(int pageSize)
    {
        var page = await GetPlantsPageAsync(1, pageSize);

        yield return page.List;
        while (page.PageCount != page.PageNo)
        {
            try
            {
                page = await GetPlantsPageAsync(page.PageNo + 1, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get page {PageNumber} of Plants list from Huawei API. Processing received plants", page.PageNo);
                yield break;
            }
            
            yield return page.List;
        }
    }
    
    public async Task<PagedData<Plant>> GetPlantsPageAsync(int pageNo, int pageSize)
    {
        var response = await _httpClient.PostAsJsonAsync("/rest/openapi/pvms/v1/plants", new GetPlantsRequest(pageNo, pageSize), CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var responseModel = await response.Content.ReadFromJsonAsync<GetPlantsResponse>();

        if (responseModel is not { Success: true } || responseModel.Data == null)
        {
            _logger.LogError("Failed to get Plants list from Huawei API. Message={Message}, FailCode={FailCode}", responseModel?.Message, responseModel?.FailCode);
        }
        
        return responseModel!.Data!;
    }
    
    
    public async Task<GetRealTimeKpiResponseInner[]> GetRealTimeKpi(string[] plantsCodes)
    {
        var request = new GetRealTimeKpiRequest(string.Join(",", plantsCodes));
        var response = await _httpClient.PostAsJsonAsync("/rest/openapi/pvms/v1/vpp/plantRealtimeKpi", request, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var responseModel = await response.Content.ReadFromJsonAsync<GetRealTimeKpiResponse>();

        if (responseModel is not { Success: true } || responseModel.Data == null)
        {
            _logger.LogError("Failed to get RealTime KPI from Huawei API. Message={Message}, FailCode={FailCode}", responseModel?.Message, responseModel?.FailCode);
        }
        
        return responseModel!.Data!;
    }
}