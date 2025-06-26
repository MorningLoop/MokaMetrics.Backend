namespace MokaMetrics.Kafka.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; } = "mokametrics-backend";
    public List<string> Topics { get; set; } = new();
    public ProducerSettings Producer { get; set; } = new();
    public ConsumerSettings Consumer { get; set; } = new();
}

public class ProducerSettings
{
    public int RetryCount { get; set; } = 3;
    public int TimeoutMs { get; set; } = 30000;
    public string Acks { get; set; } = "all";
}

public class ConsumerSettings
{
    public string AutoOffsetReset { get; set; } = "earliest";
    public bool EnableAutoCommit { get; set; } = false;
    public int SessionTimeoutMs { get; set; } = 30000;
}