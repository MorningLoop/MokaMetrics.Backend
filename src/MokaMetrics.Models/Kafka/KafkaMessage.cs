namespace MokaMetrics.Models.Kafka;

public record KafkaMessage(string Topic, string Key, string Value, DateTime Timestamp);
