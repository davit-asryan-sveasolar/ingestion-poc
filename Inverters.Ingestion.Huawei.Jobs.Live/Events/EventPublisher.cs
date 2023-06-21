using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace Inverters.Ingestion.Huawei.Jobs.Live.Events;

public class EventPublisher
{
    private readonly IAmazonSimpleNotificationService _sns;
    private const string topicArn = "arn:aws:sns:eu-west-1:824339063175:IngestionPOC-SNS-Standard";
    public EventPublisher(IAmazonSimpleNotificationService sns)
    {
        _sns = sns;
    }
    
    public async Task PublishEvent<T>(T eventToPublish)
    {
        var body = JsonSerializer.Serialize(eventToPublish);
        await _sns.PublishAsync(topicArn, body);
    }
    
    public async Task PublishBatchEvents<T>(IEnumerable<T> eventsToPublish)
    {
        if (eventsToPublish.Any())
        {
            var serialized = eventsToPublish.Select(x => new PublishBatchRequestEntry{ Message = JsonSerializer.Serialize(x), Id = Guid.NewGuid().ToString()}).ToList();
            await _sns.PublishBatchAsync(new PublishBatchRequest{ TopicArn = topicArn, PublishBatchRequestEntries = serialized });
        }
    }
}