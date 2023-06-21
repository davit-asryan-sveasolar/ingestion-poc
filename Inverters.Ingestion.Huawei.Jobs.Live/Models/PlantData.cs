using Vendors.Huawei.Client.Models;

namespace Inverters.Ingestion.Huawei.Jobs.Live.Models;

public class PlantData
{
    public PlantData(GetRealTimeKpiResponseInner realTimeData, DateTime timestamp)
    {
        RealTimeData = realTimeData;
        Timestamp = timestamp;
    }

    public GetRealTimeKpiResponseInner RealTimeData { get; }
    public DateTime Timestamp { get; }
}