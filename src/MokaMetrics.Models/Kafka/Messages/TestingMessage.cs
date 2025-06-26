namespace MokaMetrics.Models.Kafka.Messages;

public class TestingMessage : MachineKafkaMessage
{
    public Dictionary<string, bool> FunctionalTestResults { get; set; }
    public float BoilerPressure { get; set; }
    public float BoilerTemperature { get; set; }
    public float EnergyConsumption { get; set; }
}
