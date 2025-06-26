namespace MokaMetrics.Models.Kafka.Messages;

public class NewOrderLotMessage
{
    public string Site { get; set; } // italy, brazil, vietnam
    public string LotCode { get; set; }
    public DateTime LocalTimestamp { get; set; }
    public DateTime UtcTimestamp { get; set; }
    public int MachinesToProduce { get; set; }
}
