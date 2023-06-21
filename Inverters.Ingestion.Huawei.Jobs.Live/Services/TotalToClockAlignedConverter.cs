namespace Inverters.Ingestion.Huawei.Jobs.Live.Services;

public class TotalToClockAlignedConverter
{
    private readonly HuaweiSiteDataRepository _repository;

    public TotalToClockAlignedConverter(HuaweiSiteDataRepository repository)
    {
        _repository = repository;
    }

    public decimal? GetClockAligned(string siteId, decimal currentTotal, DateTime sampledAt)
    {
        var data = _repository.GetBySiteId(siteId);

        if (data.LastTotalProduction == null || data.LastTotalProductionUpdatedAt == null)
        {
            return null;
        }

        var timeDiffSeconds = (sampledAt - data.LastTotalProductionUpdatedAt.Value).TotalSeconds;
        var energyDiff = currentTotal - data.LastTotalProduction.Value;
        var averagePerSecond = energyDiff / (decimal)timeDiffSeconds;
        return averagePerSecond * 5 * 60;
    }
}