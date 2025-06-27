using Microsoft.Extensions.Logging;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Models.Kafka.Messages;
using System.Text.Json;

namespace MokaMetrics.Kafka.Consumer;
public class TopicProcessor
{
    private readonly ILogger<TopicProcessor> _logger;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IInfluxDb3Service _influx;
    public TopicProcessor(ILogger<TopicProcessor> logger, IKafkaProducer kafkaProducer, IInfluxDb3Service influx)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _influx = influx;
    }
    public async Task ProcessMessageAsync(string topic, string message)
    {
        try
        {
            var messageObj = new GeneralMessage();
            switch (topic)
            {
                case "mokametrics.telemetry.cnc":
                    _logger.LogInformation("Processing message for cnc: {Message}", message);
                    messageObj = JsonSerializer.Deserialize<CncMessage>(message);
                    break;
                case "mokametrics.telemetry.lathe":
                    
                    _logger.LogInformation("Processing message for lathe: {Message}", message);
                    
                    break;
                case "mokametrics.telemetry.assembly":
                    
                    _logger.LogInformation("Processing message for assembly: {Message}", message);
                    
                    break;
                case "mokametrics.telemetry.testing":
                    
                    _logger.LogInformation("Processing message for testing: {Message}", message);
                    
                    break;
                case "mokametrics.production.lot_completion":
                    
                    _logger.LogInformation("Processing message for lot_completion: {Message}", message);
                    
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
