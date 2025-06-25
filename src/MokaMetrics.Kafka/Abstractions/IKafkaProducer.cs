using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokaMetrics.Kafka.Abstractions
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string key, string value);
        Task ProduceAsync<T>(string topic, string key, T value) where T : class;
        Task<bool> ProduceWithRetryAsync(string topic, string key, string value, int maxRetries = 3);
    }
}
