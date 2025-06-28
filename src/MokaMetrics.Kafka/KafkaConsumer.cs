using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.Configuration;
using MokaMetrics.Kafka.Consumer;

namespace MokaMetrics.Kafka;

public class KafkaConsumer : IKafkaConsumer
{
    private IConsumer<string, string>? _consumer;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly TopicProcessor _topicProcessor;

    public KafkaConsumer(KafkaSettings settings, ILogger<KafkaConsumer> logger, TopicProcessor topicProcessor)
    {
        _settings = settings;
        _logger = logger;
        _topicProcessor = topicProcessor;
    }

    public void SubscribeToTopics()
    {
        if (_consumer == null)
        {
            throw new InvalidOperationException("Consumer is not initialized. Call InitializeConsumer first.");
        }

        if (!_settings.Topics.Any())
        {
            _logger.LogWarning("No topics configured for consumption");
            return;
        }

        _consumer!.Subscribe(_settings.Topics);
        _logger.LogInformation("Kafka consumer started for topics: {Topics}",
            string.Join(", ", _settings.Topics));
    }
    public async Task ConsumeMessageAsync(CancellationToken stoppingToken)
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

    public void InitializeConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.Host,
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

    public void Close()
    {
        _consumer?.Close();
        _logger.LogInformation("Kafka consumer closed");
    }
    public void Dispose()
    {
        _consumer?.Dispose();
        _logger.LogInformation("Kafka consumer disposed");
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
        await _topicProcessor.ProcessMessageAsync(topic, value);
    }
}
