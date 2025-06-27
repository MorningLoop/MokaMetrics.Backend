namespace MokaMetrics.Models.Kafka.Messages;

public class AssemblyMessage : MachineMessage
{
    public float AverageTimePerStation { get; set; }
    public int ActiveOperators { get; set; }
}
