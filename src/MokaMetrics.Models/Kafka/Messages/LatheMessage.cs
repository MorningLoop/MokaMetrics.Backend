namespace MokaMetrics.Models.Kafka.Messages;

public class LatheMessage : MachineMessage
{
    public float RotationSpeed { get; set; }
    public float SpindleTemperature { get; set; }
}
