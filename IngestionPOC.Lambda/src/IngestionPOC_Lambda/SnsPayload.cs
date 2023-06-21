namespace IngestionPOC_Lambda;

public class SnsPayload
{
    public string TopicArn { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
