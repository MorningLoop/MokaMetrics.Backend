using Confluent.Kafka;

namespace MokaMetrics.Services.ServicesInterfaces
{
    public interface IKafkaService
    {
        Message<Ignore, string> GetValueTopicBrasil();
    }
}