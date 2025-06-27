using System.Text.Json;

namespace MokaMetrics.Models.Kafka.Messages.Base;

public class GeneralMessage
{
    public DateTime? LocalTimestamp { get; set; }
    public DateTime? UtcTimestamp { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
