using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.Configuration;
using System.Text.Json;

namespace MokaMetrics.Kafka
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSettings _settings;
        private readonly ILogger<KafkaProducer> _logger;
        private bool _disposed = false;

        public KafkaProducer(KafkaSettings settings, ILogger<KafkaProducer> logger)
        {
            _settings = settings;
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings.Host,
                Acks = Enum.Parse<Acks>(_settings.Producer.Acks, true),
                MessageTimeoutMs = _settings.Producer.TimeoutMs,
                EnableIdempotence = true,
                RetryBackoffMs = 1000,
                MessageSendMaxRetries = _settings.Producer.RetryCount
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka Producer Error: {Error}", e.Reason))
                .Build();
        }

        public async Task ProduceAsync(string topic, string key, string value)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = key,
                    Value = value,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };

                var result = await _producer.ProduceAsync(topic, message);

                _logger.LogInformation(
                    "Message produced to {Topic} partition {Partition} at offset {Offset}",
                    result.Topic, result.Partition.Value, result.Offset.Value);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Failed to produce message to topic {Topic}", topic);
                throw;
            }
        }

        public async Task ProduceAsync<T>(string topic, string key, T value) where T : class
        {
            var json = JsonSerializer.Serialize(value);
            await ProduceAsync(topic, key, json);
        }

        public async Task<bool> ProduceWithRetryAsync(string topic, string key, string value, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await ProduceAsync(topic, key, value);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Attempt {Attempt}/{MaxRetries} failed for topic {Topic}: {Error}",
                        attempt, maxRetries, topic, ex.Message);

                    if (attempt == maxRetries)
                    {
                        _logger.LogError("All retry attempts exhausted for topic {Topic}", topic);
                        return false;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _producer?.Flush(TimeSpan.FromSeconds(10));
                _producer?.Dispose();
                _disposed = true;
            }
        }
    }
}
