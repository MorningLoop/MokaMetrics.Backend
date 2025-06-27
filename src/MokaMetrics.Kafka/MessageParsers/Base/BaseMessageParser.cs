using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Models.Kafka.Messages.Base;
using System.Text.Json;

namespace MokaMetrics.Kafka.MessageParsers.Base;

public abstract class BaseMessageParser<TMessage> : IMessageParser where TMessage : GeneralMessage
{
    protected abstract string SupportedTopic { get; }

    public bool CanParse(string topic) => topic == SupportedTopic;

    public T Parse<T>(string message) where T : GeneralMessage
    {
        if (typeof(T) != typeof(TMessage) && typeof(T) != typeof(GeneralMessage))
        {
            throw new InvalidOperationException($"This parser can only parse {typeof(TMessage).Name}");
        }
        
        var result = JsonSerializer.Deserialize<TMessage>(message, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        return (T)(GeneralMessage)result!;
    }
    
    public Type GetMessageType() => typeof(TMessage);
}