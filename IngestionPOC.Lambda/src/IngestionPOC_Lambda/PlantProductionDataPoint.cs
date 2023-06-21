namespace IngestionPOC_Lambda;

public class PlantProductionDataPoint
{
    public PlantProductionDataPoint(string plantId, decimal value, DateTime timestamp)
    {
        PlantId = plantId;
        Value = value;
        Timestamp = timestamp;
    }

    public string PlantId { get; }
    public decimal Value { get; }
    public DateTime Timestamp { get; }
}