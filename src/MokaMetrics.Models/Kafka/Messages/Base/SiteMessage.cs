using System.Text.Json;

namespace MokaMetrics.Models.Kafka.Messages.Base;

public class SiteMessage : GeneralMessage
{   
    public string Site { get; set; } // italy, brazil, vietnam
    public string LotCode { get; set; }
}
