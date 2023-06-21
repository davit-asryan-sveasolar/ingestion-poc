using System.Text.Json;
using Inverters.Ingestion.Huawei.Jobs.Live.Models;

namespace Inverters.Ingestion.Huawei.Jobs.Live.Services;

public class HuaweiSiteDataRepository
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "siteData.json");
    private readonly Dictionary<string, HuaweiSiteData> _data;
    private readonly Dictionary<string, HuaweiSiteData> _dataNew;

    public HuaweiSiteDataRepository()
    {
        _data = File.Exists(FilePath) ? JsonSerializer.Deserialize<HuaweiSiteData[]>(File.ReadAllText(FilePath))!.ToDictionary(x => x.SiteId, x => x) : new Dictionary<string,HuaweiSiteData>();
        _dataNew = new Dictionary<string, HuaweiSiteData>(_data);
    }
    
    public HuaweiSiteData GetBySiteId(string siteId)
    {
        return _data.TryGetValue(siteId, out var data) ? data : new HuaweiSiteData(siteId, null, null);
    }

    public void Update(HuaweiSiteData data)
    {
        _dataNew[data.SiteId] = data;
    }

    public async Task FlushAsync()
    {
        await File.WriteAllTextAsync(FilePath, JsonSerializer.Serialize(_dataNew.Values));
    }
}