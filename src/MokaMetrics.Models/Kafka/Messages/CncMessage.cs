namespace MokaMetrics.Models.Kafka.Messages;

public class CncMessage : MachineMessage
{
    public float CycleTime { get; set; }
    public float CuttingDepth { get; set; }
    public float Vibration { get; set; }
    public bool Alarm { get; set; }
}
