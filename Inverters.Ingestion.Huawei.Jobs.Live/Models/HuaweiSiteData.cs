namespace Inverters.Ingestion.Huawei.Jobs.Live.Models;

public class HuaweiSiteData
{
    public HuaweiSiteData(string siteId, decimal? lastTotalProduction, DateTime? lastTotalProductionUpdatedAt)
    {
        SiteId = siteId;
        LastTotalProduction = lastTotalProduction;
        LastTotalProductionUpdatedAt = lastTotalProductionUpdatedAt;
    }

    public string SiteId { get; }
    public decimal? LastTotalProduction { get; }
    public DateTime? LastTotalProductionUpdatedAt { get; }
}