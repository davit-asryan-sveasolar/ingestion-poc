namespace Vendors.Huawei.Client.Models;

public class GetRealTimeKpiResponseInner
{
    public GetRealTimeKpiResponseInner(PlantRealTimeData dataItemMap, string plantCode)
    {
        DataItemMap = dataItemMap;
        PlantCode = plantCode;
    }

    public PlantRealTimeData DataItemMap { get; }
    public string PlantCode { get; }
}