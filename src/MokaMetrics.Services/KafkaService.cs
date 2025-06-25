using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using MokaMetrics.Services.ServicesInterfaces;

namespace MokaMetrics.Services
{
    public class KafkaService : IKafkaService, IDisposable
    {
        private ConsumerConfig _consumerConfig;
        private string? _brasilTopic;
        private readonly IConsumer<string, string> _consumer;
        private bool _isSubscribed = false;

        public KafkaService(IConfiguration configuration)
        {
            Random rd = new Random();
            _brasilTopic = configuration.GetSection("Kafka:TopicBrasil").Value;
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = configuration.GetSection("Kafka:Host").Value,
                GroupId = rd.Next().ToString(),
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false,
            };

            _consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .SetErrorHandler((_, e) => Console.WriteLine($"Errore consumer Kafka: {e.Reason}"))
                .Build();
        }

        public async Task<string?> GetLatestMessageAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_isSubscribed)
                {
                    _consumer.Subscribe(_brasilTopic);
                    _isSubscribed = true;
                    Console.WriteLine($"Sottoscritto al topic: {_brasilTopic}");
                }

                var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(1000));
                if (consumeResult != null && consumeResult.Message != null)
                {
                    Console.WriteLine($"Messaggio ricevuto - Key: {consumeResult.Message.Key ?? "null"}, Value: {consumeResult.Message.Value}");
                    _consumer.Commit(consumeResult);
                    return consumeResult.Message.Value;
                }
                return null;
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"Errore durante il consume Kafka: {ex.Error.Reason}");
                return null;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operazione Kafka cancellata");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore generico durante il consume Kafka: {ex.Message}");
                return null;
            }
        }

        public string? GetValueTopicBrasil()
        {
            return GetLatestMessageAsync().GetAwaiter().GetResult();
        }

        public async Task<string?> GetLastAvailableMessageAsync()
        {
            try
            {
                var config = new ConsumerConfig()
                {
                    BootstrapServers = _consumerConfig.BootstrapServers,
                    GroupId = _consumerConfig.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    EnableAutoCommit = false,
                };

                using var tempConsumer = new ConsumerBuilder<string, string>(config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .SetErrorHandler((_, e) => Console.WriteLine($"Errore temp consumer Kafka: {e.Reason}"))
                    .Build();
                var partition = new TopicPartition(_brasilTopic, 0);
                var watermarkOffsets = tempConsumer.GetWatermarkOffsets(partition);
                if (watermarkOffsets.High.Value > 0)
                {
                    var lastOffset = new TopicPartitionOffset(partition, watermarkOffsets.High.Value - 1);
                    tempConsumer.Assign(new[] { lastOffset });
                    var result = tempConsumer.Consume(TimeSpan.FromSeconds(5));
                    return result?.Message?.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la lettura dell'ultimo messaggio: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
        }
    }
}
