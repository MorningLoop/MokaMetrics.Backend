
namespace MokaMetrics.Kafka.Abstractions;

public interface IKafkaConsumer
{
    Task ConsumeMessageAsync(CancellationToken stoppingToken);
    void InitializeConsumer();
    void SubscribeToTopics();
    void Close();
    void Dispose();
}
