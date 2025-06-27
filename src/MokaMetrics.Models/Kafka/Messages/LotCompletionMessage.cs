using MokaMetrics.Models.Kafka.Messages.Base;

namespace MokaMetrics.Models.Kafka.Messages;

public class LotCompletionMessage : SiteMessage
{
    public int LotTotalQuantity { get; set; }
    public int LotProducedQuantity { get; set; }
    public int CncDuration { get; set; }
    public int LatheDuration { get; set; }
    public int AssemblyDuration { get; set; }
    public int TestDuration { get; set; }
}
