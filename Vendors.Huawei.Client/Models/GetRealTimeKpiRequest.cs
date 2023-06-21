namespace Vendors.Huawei.Client.Models;

public class GetRealTimeKpiRequest
{
    public GetRealTimeKpiRequest(string plantCodes)
    {
        PlantCodes = plantCodes;
    }

    public string PlantCodes { get; }
}