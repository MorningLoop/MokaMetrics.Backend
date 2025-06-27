using MokaMetrics.Models.Kafka.Messages.Base;

namespace MokaMetrics.Models.Kafka.Messages;

public class MachineMessage : SiteMessage
{
    public string MachineId { get; set; }
    public int Status { get; set; }
    public int CompletedPiecesFromLastMaintenance { get; set; }
    public string? Error { get; set; }
}
