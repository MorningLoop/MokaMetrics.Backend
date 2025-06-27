using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Kafka.Messages;
using System.Text.Json;

namespace MokaMetrics.Kafka.MessageParsers;

public class LatheMessageParser : BaseMessageParser<LatheMessage>
{
    protected override string SupportedTopic => "mokametrics.telemetry.lathe";
}