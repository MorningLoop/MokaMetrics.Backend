using System.Text.Json;

namespace MokaMetrics.Models.Kafka.Messages;

public class GeneralMessage
{   
    public string Site { get; set; } // italy, brazil, vietnam
    public string LotCode { get; set; }
    public DateTime? LocalTimestamp { get; set; }
    public DateTime? UtcTimestamp { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
