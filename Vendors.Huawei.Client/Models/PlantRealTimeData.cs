namespace Vendors.Huawei.Client.Models;

public class PlantRealTimeData
{
    public PlantRealTimeData(decimal totalEnergy)
    {
        TotalEnergy = totalEnergy;
    }

    public decimal TotalEnergy { get; }
}