using MokaMetrics.Models.Kafka.Messages.Base;

namespace MokaMetrics.Kafka.Abstractions;

public interface IMessageParser
{
    bool CanParse(string topic);
    T Parse<T>(string message) where T : GeneralMessage;
    Type GetMessageType();
}