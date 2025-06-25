namespace MokaMetrics.Models.Kafka.Messages;

public class AssemblyMessage : MachineKafkaMessage
{
    public float AverageTimePerStation { get; set; }
    public int ActiveOperators { get; set; }
}
