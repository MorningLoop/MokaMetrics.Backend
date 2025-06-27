using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Kafka.Messages;
using System.Text.Json;

namespace MokaMetrics.Kafka.MessageParsers;

public class CncMessageParser : BaseMessageParser<CncMessage>
{
    protected override string SupportedTopic => "mokametrics.telemetry.cnc";
}