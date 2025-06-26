using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MokaMetrics.Kafka.Configuration;

namespace MokaMetrics.Kafka.Consumer;

public class KafkaConsumerService : BackgroundService
{
    private IConsumer<string, string>? _consumer;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(KafkaSettings settings, ILogger<KafkaConsumerService> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service is starting...");
        
        try
        {
            InitializeConsumer();

            if (!_settings.Topics.Any())
            {
                _logger.LogWarning("No topics configured for consumption");
                return;
            }

            _consumer!.Subscribe(_settings.Topics);
            _logger.LogInformation("Kafka consumer started for topics: {Topics}",
                string.Join(", ", _settings.Topics));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult?.Message != null)
                    {
                        await ProcessMessageAsync(consumeResult);

                        // Manual commit after successful processing
                        if (!_settings.Consumer.EnableAutoCommit)
                        {
                            _consumer.Commit(consumeResult);
                        }
                    }
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
    
    private void InitializeConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_settings.Consumer.AutoOffsetReset, true),
            EnableAutoCommit = _settings.Consumer.EnableAutoCommit,
            SessionTimeoutMs = _settings.Consumer.SessionTimeoutMs,
            EnablePartitionEof = false
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka Consumer Error: {Error}", e.Reason))
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                _logger.LogInformation("Assigned partitions: [{Partitions}]",
                    string.Join(", ", partitions.Select(p => $"{p.Topic}:{p.Partition}")));
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                _logger.LogInformation("Revoked partitions: [{Partitions}]",
                    string.Join(", ", partitions.Select(p => $"{p.Topic}:{p.Partition}")));
            })
            .Build();
    }

    private async Task ProcessMessageAsync(ConsumeResult<string, string> consumeResult)
    {
        var message = consumeResult.Message;

        _logger.LogInformation(
            "Consumed message from {Topic}:{Partition}:{Offset} - Key: {Key}, Value: {Value}",
            consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value,
            message.Key, message.Value);

        // Process the message based on topic
        try
        {
            await ProcessByTopic(consumeResult.Topic, message.Key, message.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from topic {Topic}", consumeResult.Topic);

            // Here you could implement dead letter queue logic
            // or other error handling strategies
            throw; // Re-throw to prevent commit if auto-commit is disabled
        }
    }

    private async Task ProcessByTopic(string topic, string key, string value)
    {
        // Route message processing based on topic
        switch (topic.ToLowerInvariant())
        {
            case "user-events":
                await ProcessUserEvent(key, value);
                break;
            case "order-events":
                await ProcessOrderEvent(key, value);
                break;
            default:
                _logger.LogInformation("Processing generic message from topic {Topic}", topic);
                // Generic processing logic
                break;
        }
    }

    private async Task ProcessUserEvent(string key, string value)
    {
        // Example: Process user-related events
        _logger.LogInformation("Processing user event - Key: {Key}", key);

        // Here you would implement your business logic
        // For example: deserialize JSON, call services, update database, etc.

        await Task.Delay(100); // Simulate processing time
    }

    private async Task ProcessOrderEvent(string key, string value)
    {
        // Example: Process order-related events
        _logger.LogInformation("Processing order event - Key: {Key}", key);

        // Business logic for order events
        await Task.Delay(100); // Simulate processing time
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
