using MokaMetrics.Models.Kafka.Messages.Base;
using System.Text.Json;

namespace MokaMetrics.Models.Kafka.Messages;

public class NewOrderLotMessage : GeneralMessage
{
    public string Customer { get; set; }
    public int QuantityMachines { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime Deadline { get; set; }
    public List<LotMessage> Lots { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class LotMessage
{
    public string LotCode { get; set; }
    public int TotalQuantity { get; set; }
    public DateTime StartDate { get; set; }
    public string IndustrialFacility { get; set; }
}
