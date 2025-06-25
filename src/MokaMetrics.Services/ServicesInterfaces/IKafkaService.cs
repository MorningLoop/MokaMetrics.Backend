using Confluent.Kafka;

namespace MokaMetrics.Services.ServicesInterfaces
{
    public interface IKafkaService
    {
        string? GetValueTopicBrasil();
        Task<string?> GetLatestMessageAsync(CancellationToken cancellationToken = default);
        Task<string?> GetLastAvailableMessageAsync(); // Nuovo metodo per ottenere l'ultimo messaggio disponibile
    }
}