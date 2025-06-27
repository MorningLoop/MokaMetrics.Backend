using System.Text.Json;

namespace MokaMetrics.Models.Kafka.Messages;

public class MachineKafkaMessage : SiteMessage
{
    public int MachineId { get; set; }
    public int Status { get; set; }
    public int CompletedPiecesFromLastMaintenance { get; set; }
    public string? Error { get; set; }
}
