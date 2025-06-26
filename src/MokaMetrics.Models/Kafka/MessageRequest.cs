namespace MokaMetrics.Models.Kafka;

public record MessageRequest(string Topic, string Key, string Value);
