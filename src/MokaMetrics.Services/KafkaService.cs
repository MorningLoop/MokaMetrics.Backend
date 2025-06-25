
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using MokaMetrics.Services.ServicesInterfaces;

namespace MokaMetrics.Services
{
    public class KafkaService : IKafkaService
    {
        private ConsumerConfig _producerConfig;
        private string? _brasilTopic;
        public KafkaService(IConfiguration configuration)
        {
            _brasilTopic = configuration.GetSection("Kafka:TopicBrasil").Value;
            _producerConfig = new ConsumerConfig()
            {
                BootstrapServers = configuration.GetSection("Kafka:Host").Value,
                GroupId = configuration.GetSection("Kafka:ConsumerGroupId").Value,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public Message<Ignore, string> GetValueTopicBrasil()
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_producerConfig).Build())
            {
                consumer.Subscribe(_brasilTopic);
                ConsumeResult<Ignore,string>? consumeResult = consumer.Consume(cancellationToken: CancellationToken.None);
                consumer.Close();
                return consumeResult.Message;
            }
        }
    }

}
