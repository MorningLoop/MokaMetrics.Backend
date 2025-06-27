using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Kafka.Messages;
using System.Text.Json;

namespace MokaMetrics.Kafka.MessageParsers;

public class TestingMessageParser : BaseMessageParser<TestingMessage>
{
    protected override string SupportedTopic => "mokametrics.telemetry.testing";
}