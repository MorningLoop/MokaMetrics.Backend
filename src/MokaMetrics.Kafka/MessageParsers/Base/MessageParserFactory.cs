using MokaMetrics.Models.Kafka.Messages;
using Microsoft.Extensions.Logging;
using MokaMetrics.Kafka.Abstractions;

namespace MokaMetrics.Kafka.MessageParsers.Base;

public class MessageParserFactory
{
    private readonly IEnumerable<IMessageParser> _parsers;
    private readonly ILogger<MessageParserFactory> _logger;

    public MessageParserFactory(IEnumerable<IMessageParser> parsers, ILogger<MessageParserFactory> logger)
    {
        _parsers = parsers;
        _logger = logger;
    }

    public T ParseMessage<T>(string topic, string message) where T : GeneralMessage
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(topic));
        
        if (parser != null)
        {
            try
            {
                return parser.Parse<T>(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing message from topic {Topic}", topic);
            }
        }
        
        _logger.LogWarning("No parser found for topic: {Topic}", topic);
        return Activator.CreateInstance<T>();
    }
    
    public Type GetMessageType(string topic)
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(topic));
        return parser?.GetMessageType() ?? typeof(GeneralMessage);
    }
}