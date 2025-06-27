using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Kafka.Messages;
using System.Text.Json;

namespace MokaMetrics.Kafka.MessageParsers;

public class AssemblyMessageParser : BaseMessageParser<AssemblyMessage>
{
    protected override string SupportedTopic => "mokametrics.telemetry.assembly";
}