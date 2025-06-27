using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Kafka.Messages;

namespace MokaMetrics.Kafka.MessageParsers;

public class NewOrderLotMessageParser : BaseMessageParser<NewOrderLotMessage>
{
    protected override string SupportedTopic => "mokametrics.order";
}
