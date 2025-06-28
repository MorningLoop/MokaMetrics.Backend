using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.Configuration;
using static Confluent.Kafka.ConfigPropertyNames;

namespace MokaMetrics.Kafka.Consumer;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IKafkaConsumer _consumer;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IKafkaConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service is starting...");
        
        try
        {
            // starting consumer
            _consumer.InitializeConsumer();
            _consumer.SubscribeToTopics();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _consumer.ConsumeMessageAsync(stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Consume error: {Error}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in consumer loop");
                    await Task.Delay(5000, stoppingToken); // Wait before retrying
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Kafka Consumer Service");
        }
        finally
        {
            _consumer?.Close();
            _logger.LogInformation("Kafka consumer stopped");
        }
    }
    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
