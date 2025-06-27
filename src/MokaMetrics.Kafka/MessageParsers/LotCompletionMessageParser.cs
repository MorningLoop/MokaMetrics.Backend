using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Kafka.Messages;

namespace MokaMetrics.Kafka.MessageParsers;

public class LotCompletionMessageParser : BaseMessageParser<LotCompletionMessage>
{
    protected override string SupportedTopic => "mokametrics.production.lot_completion";
}
