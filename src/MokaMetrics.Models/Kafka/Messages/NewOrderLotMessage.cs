namespace MokaMetrics.Models.Kafka.Messages;

public class NewOrderLotMessage : GeneralMessage
{
    public int MachinesToProduce { get; set; }
}
