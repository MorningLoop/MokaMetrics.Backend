namespace MokaMetrics.Models.Kafka.Messages;

public class LatheMessage : MachineKafkaMessage
{
    public float RotationSpeed { get; set; }
    public float SpindleTemperature { get; set; }
}
