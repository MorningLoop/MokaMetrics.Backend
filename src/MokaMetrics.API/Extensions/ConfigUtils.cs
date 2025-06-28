using MokaMetrics.Kafka.Configuration;
using System.Globalization;

namespace MokaMetrics.API.Extensions;

public class ConfigUtils
{
    private readonly IConfiguration _configuration;
    public ConfigUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public KafkaSettings GetKafkaProducerSettings()
    {
        var section = _configuration.GetSection("Kafka:Producer");
        var kafkaSettings = new KafkaSettings()
        {
            Host = section["Host"] ?? "localhost:9092",
            GroupId = section["GroupId"] ?? "mokametrics-producer-backend",
            Producer = new ProducerSettings
            {
                RetryCount = int.Parse(section["RetryCount"] ?? "3"),
                TimeoutMs = int.Parse(section["TimeoutMs"] ?? "30000"),
                Acks = section["Acks"] ?? "all"
            }
        };
        
        if (kafkaSettings is null)
        {
            throw new ArgumentException("KafkaProducer settings are null");
        }
        return kafkaSettings;
    }

    public KafkaSettings GetKafkaConsumerSettings(KafkaConsumerSettingsType type)
    {
        var section = _configuration.GetSection($"Kafka:Consumer:{type.ToString()}");
        var kafkaSettings = new KafkaSettings()
        {
            Host = section["Host"] ?? "localhost:9092",
            GroupId = section["GroupId"] ?? "mokametrics-backend",
            Topics = section["Topics"]?.Split("|").ToList() ?? new List<string>(),
            Consumer = new ConsumerSettings
            {
                AutoOffsetReset = section["AutoOffsetReset"] ?? "earliest",
                EnableAutoCommit = bool.Parse(section["EnableAutoCommit"] ?? "false"),
                SessionTimeoutMs = int.Parse(section["SessionTimeoutMs"] ?? "30000")
            }
        };

        if (kafkaSettings is null)
        {
            throw new ArgumentException("KafkaConsumer for backend settings are required");
        }
        return kafkaSettings;
    }
}

public enum KafkaConsumerSettingsType
{
    Backend,
    Frontend
}
