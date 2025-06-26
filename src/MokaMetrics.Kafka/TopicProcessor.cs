using Microsoft.Extensions.Logging;
using MokaMetrics.Kafka.Abstractions;

namespace MokaMetrics.Kafka.Consumer;
public class TopicProcessor
{
    private readonly ILogger<TopicProcessor> _logger;
    private IKafkaProducer _kafkaProducer;
    public TopicProcessor(ILogger<TopicProcessor> logger, IKafkaProducer kafkaProducer)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }
    public async Task ProcessMessageAsync(string topic, string message)
    {
        try
        {
            switch (topic)
            {
                case "orders":
                    // Process message for topic1
                    _logger.LogInformation("Processing message for topic1: {Message}", message);
                    // Add your processing logic here
                    break;
                case "topic2":
                    // Process message for topic2
                    _logger.LogInformation("Processing message for topic2: {Message}", message);
                    // Add your processing logic here
                    break;
                default:
                    _logger.LogWarning("No processing logic defined for topic: {Topic}", topic);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from topic {Topic}: {Message}", topic, message);
            throw;
        }
    }
}
