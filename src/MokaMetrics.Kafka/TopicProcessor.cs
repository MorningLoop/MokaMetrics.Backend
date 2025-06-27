using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.Models.Helpers;
using MokaMetrics.Models.Influx;
using MokaMetrics.Models.Kafka.Messages;
using System.Reflection;
using System.Text.Json;

namespace MokaMetrics.Kafka.Consumer;

public class TopicProcessor
{
    private readonly ILogger<TopicProcessor> _logger;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IInfluxDb3Service _influx;
    private readonly MessageParserFactory _messageParserFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TopicProcessor(
        ILogger<TopicProcessor> logger,
        IKafkaProducer kafkaProducer,
        IInfluxDb3Service influx,
        MessageParserFactory messageParserFactory,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _influx = influx;
        _messageParserFactory = messageParserFactory;
        _serviceScopeFactory = scopeFactory;
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
                    await ProcessStatus(latheMessage.Site, "lathe", latheMessage.Error);
                    break;
                case AssemblyMessage assemblyMessage:
                    await ProcessAssemblyMessageAsync(assemblyMessage);
                    await ProcessStatus(assemblyMessage.Site, "assembly", assemblyMessage.Error);
                    break;
                case TestingMessage testingMessage:
                    await ProcessTestingMessageAsync(testingMessage);
                    await ProcessStatus(testingMessage.Site, "testing", testingMessage.Error);
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
        await ProcessMeasurement(message);
        _logger.LogInformation("Finished processing CNC message...");
    }

    private async Task ProcessLatheMessageAsync(LatheMessage message)
    {
        _logger.LogInformation("Processing Lathe message...");
        await ProcessMeasurement(message);
        _logger.LogInformation("Finished processing Lathe message...");
    }

    private async Task ProcessAssemblyMessageAsync(AssemblyMessage message)
    {
        _logger.LogInformation("Processing Assembly message...");
        await ProcessMeasurement(message);
        _logger.LogInformation("Finished processing Assembly message...");
    }

    private async Task ProcessTestingMessageAsync(TestingMessage message)
    {
        _logger.LogInformation("Processing Testing message...");
        await ProcessMeasurement(message);
        _logger.LogInformation("Finished processing Testing message...");
    }

    private async Task ProcessNewOrderLotMessageAsync(NewOrderLotMessage message)
    {
        _logger.LogInformation("Processing NewOrderLot Message...");
        await ProcessMeasurement(message);
        _logger.LogInformation("Finished processing NewOrderLot message...");
    }

    private async Task ProcessLotCompletionMessageAsync(LotCompletionMessage message)
    {
        _logger.LogInformation("Processing LotCompletion Message...");

        using var scope = _serviceScopeFactory.CreateScope();

        var _uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var lot = await _uow.Lots.GetByCodeAsync(message.LotCode);

        if (lot == null)
        {
            _logger.LogWarning("Lot with code {LotCode} not found", message.LotCode);
            return;
        }

        lot.UpdatedAt = DateTime.UtcNow;
        lot.ManufacturedQuantity = message.LotProducedQuantity;

        if (lot.ManufacturedQuantity == lot.TotalQuantity)
        {
            var order = await _uow.Orders.GetOrderWithLotsAsync(lot.OrderId);
            if (order.Lots.Where(l => l.Id != lot.Id).All(x => x.ManufacturedQuantity == x.TotalQuantity))
            {
                order.FullfilledDate = DateTime.UtcNow;
                _logger.LogInformation("Order {OrderId} is fully fulfilled", order.Id);
            }
        }

        await _uow.SaveChangesAsync();

        _logger.LogInformation("Finished processing LotCompletion message...");
    }

    private async Task ProcessMeasurement(dynamic message)
    {
        try
        {
            var batch = new List<TimeSeriesData>();

            foreach (PropertyInfo property in message.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType == typeof(Dictionary<string, bool>))
                {
                    string serializedDict = JsonSerializer.Serialize(property.GetValue(message), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

                    batch.Add(new TimeSeriesData()
                    {
                        Measurement = property.Name,
                        Fields = new Dictionary<string, object>()
                        {
                            { "value", serializedDict },
                        },
                        Tags = new Dictionary<string, string>()
                        {
                            { "location", message.Site.ToLower() },
                            { "machine", message.MachineId},
                            { "lot_code", message.LotCode },
                            { "local_timestamp", message.LocalTimestamp?.ToString("o") }
                        },
                        Timestamp = message.UtcTimestamp
                    });
                }
                else
                {
                    batch.Add(new TimeSeriesData()
                    {
                        Measurement = property.Name,
                        Fields = new Dictionary<string, object>()
                        {
                            { "value", property.GetValue(message) ?? 0 },
                        },
                        Tags = new Dictionary<string, string>()
                        {
                            { "location", message.Site.ToLower() },
                            { "machine", message.MachineId},
                            { "lot_code", message.LotCode },
                            { "local_timestamp", message.LocalTimestamp?.ToString("o") }
                        },
                        Timestamp = message.UtcTimestamp
                    });
                }
            }

            await _influx.WriteDataAsync(batch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing measurement: {message.ToString()}");
            throw;
        }
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
                    { "value", error != "None" ? MachineStatuses.Alarm : MachineStatuses.Operational  }
                },
                Tags = new Dictionary<string, string>
                {
                    { "location", location.ToLower() },
                    { "machine", machine.ToLower() }
                },
                Timestamp = DateTime.UtcNow
            };

            if (error != "None")
            {
                statusTsData.Fields.Add("error_message", error);
            }

            await _influx.WriteDataAsync(statusTsData);

            _logger.LogInformation($"Finished processing status for {machine} machine in location {location}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing status for {machine} machine in location {location}");
            throw;
        }
    }
}