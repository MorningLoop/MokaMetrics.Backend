using Microsoft.Extensions.Logging;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Influx;
using MokaMetrics.Models.Kafka.Messages;
using System.Reflection;

namespace MokaMetrics.Kafka.Consumer;

public class TopicProcessor
{
    private readonly ILogger<TopicProcessor> _logger;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IInfluxDb3Service _influx;
    private readonly MessageParserFactory _messageParserFactory;

    public TopicProcessor(
        ILogger<TopicProcessor> logger,
        IKafkaProducer kafkaProducer,
        IInfluxDb3Service influx,
        MessageParserFactory messageParserFactory)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _influx = influx;
        _messageParserFactory = messageParserFactory;
    }

    public async Task ProcessMessageAsync(string topic, string message)
    {
        try
        {
            _logger.LogInformation("Processing message for topic {Topic}", topic);

            // Determine the correct message type based on the topic
            var messageType = _messageParserFactory.GetMessageType(topic);

            // Use reflection to call the correct generic method
            var method = typeof(MessageParserFactory)
                .GetMethod(nameof(MessageParserFactory.ParseMessage), new[] { typeof(string), typeof(string) })
                .MakeGenericMethod(messageType);

            var messageObj = method.Invoke(_messageParserFactory, new object[] { topic, message });

            // Process based on the specific message type
            switch (messageObj)
            {
                case CncMessage cncMessage:
                    await ProcessCncMessageAsync(cncMessage);
                    await ProcessStatus(cncMessage.Site, "cnc", cncMessage.Error);
                    break;
                case LatheMessage latheMessage:
                    await ProcessLatheMessageAsync(latheMessage);
                    await ProcessStatus(latheMessage.Site, "cnc", latheMessage.Error);
                    break;
                case AssemblyMessage assemblyMessage:
                    await ProcessAssemblyMessageAsync(assemblyMessage);
                    break;
                case TestingMessage testingMessage:
                    await ProcessTestingMessageAsync(testingMessage);
                    break;
                case LotCompletionMessage lotCompletionMessage:
                    await ProcessLotCompletionMessageAsync(lotCompletionMessage);
                    break;
                default:
                    _logger.LogWarning("No processing logic implemented for message type: {Type}", messageObj.GetType().Name);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from topic {Topic}", topic);
            throw;
        }
    }

    private async Task ProcessCncMessageAsync(CncMessage message)
    {
        _logger.LogInformation("Processing CNC message...");

        try
        {
            foreach (PropertyInfo property in typeof(CncMessage).GetProperties())
            {
                var tsData = new TimeSeriesData()
                {
                    Measurement = property.Name,
                    Fields = new Dictionary<string, object>()
                    {
                        { "value", property.GetValue(message) ?? 0 },
                    },
                    Tags = new Dictionary<string, string>()
                    {
                        { "location", message.Site.ToLower() },
                        { "machine", "cnc" },
                        { "lot_code", message.LotCode },
                        { "local_timestamp", message.LocalTimestamp?.ToString("o") ?? DateTime.UtcNow.ToString("o") }
                    },
                    Timestamp = message.UtcTimestamp ?? DateTime.UtcNow
                };
            }
            _logger.LogInformation("Finished processing CNC message...");
        }
        catch (Exception)
        {

            throw;
        }
    }

    private async Task ProcessLatheMessageAsync(LatheMessage message)
    {
        _logger.LogInformation("Processing Lathe message...");

        _logger.LogInformation("Finished processing Lathe message...");
    }

    private async Task ProcessAssemblyMessageAsync(AssemblyMessage message)
    {
        _logger.LogInformation("Processing Assembly message...");

        _logger.LogInformation("Finished processing Assembly message...");
    }

    private async Task ProcessTestingMessageAsync(TestingMessage message)
    {
        _logger.LogInformation("Processing Testing message...");

        _logger.LogInformation("Finished processing Testing message...");
    }

    private async Task ProcessNewOrderLotMessageAsync(NewOrderLotMessage message)
    {
        _logger.LogInformation("Processing NewOrderLot Message...");



        _logger.LogInformation("Finished processing NewOrderLot message...");
    }

    private async Task ProcessLotCompletionMessageAsync(LotCompletionMessage message)
    {
        _logger.LogInformation("Processing NewOrderLot Message...");



        _logger.LogInformation("Finished processing NewOrderLot message...");
    }

    private async Task ProcessStatus(string location, string machine, string error)
    {
        _logger.LogInformation($"Processing status for {machine} machine in location {location}");
        try
        {
            var statusTsData = new TimeSeriesData
            {
                Measurement = "status",
                Fields = new Dictionary<string, object>
                {
                    { "value", !string.IsNullOrEmpty(error) }
                },
                Tags = new Dictionary<string, string>
                {
                    { "location", location.ToLower() },
                    { "machine", machine.ToLower() }
                },
                Timestamp = DateTime.UtcNow
            };

            if (!string.IsNullOrEmpty(error))
            {
                statusTsData.Fields.Add("error_message", error);
            }

            await _influx.WriteDataAsync(statusTsData);

            _logger.LogInformation($"Finished processing status for {machine} machine in location {location}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing status for {machine} machine in location {location}", ex.Message);
            throw;
        }
    }
}