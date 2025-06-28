using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Models.Influx;
using MokaMetrics.Models.Kafka;

namespace MokaMetrics.API.Endpoints;

public static class TestEndpoints
{
    public static IEndpointRouteBuilder MapTestEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/test")
            .WithTags("Time Series Data (InfluxDB 3.0)");

        // Write single data point
        group.MapPost("/iot", async (TimeSeriesData data, IInfluxDb3Service influxDb) =>
        {
            await influxDb.WriteDataAsync(data);
            return Results.Ok(new { message = "Data written successfully" });
        })
        .WithName("WriteTimeSeriesDataV3")
        .WithSummary("Write a single time series data point to InfluxDB 3.0");

        // Write multiple data points
        group.MapPost("/iot/batch", async (IEnumerable<TimeSeriesData> data, IInfluxDb3Service influxDb) =>
        {
            await influxDb.WriteDataAsync(data);
            return Results.Ok(new { message = $"{data.Count()} data points written successfully" });
        })
        .WithName("WriteBatchTimeSeriesDataV3")
        .WithSummary("Write multiple time series data points to InfluxDB 3.0");

        // Query data with structured request
        group.MapPost("/iot/query", async (QueryRequest request, IInfluxDb3Service influxDb) =>
        {
            var results = await influxDb.QueryDataAsync(request);
            return Results.Ok(results);
        })
        .WithName("QueryTimeSeriesDataV3")
        .WithSummary("Query time series data using structured request");

        // Health check
        group.MapGet("/iot/health", async (IInfluxDb3Service influxDb) =>
        {
            Console.WriteLine("entered /health endpoint");
            var isHealthy = await influxDb.HealthCheckAsync();
            return isHealthy ? Results.Ok(new { status = "healthy" }) : Results.Problem("InfluxDB 3.0 is unhealthy");
        })
        .WithName("InfluxDb3HealthCheck")
        .WithSummary("Check InfluxDB 3.0 health");

        // Get latest data for a measurement
        group.MapGet("/iot/latest/{measurement}", async (string measurement, IInfluxDb3Service influxDb, int limit = 10) =>
        {
            var request = new QueryRequest
            {
                Measurement = measurement,
                StartTime = DateTime.UtcNow.AddHours(-24),
                Limit = limit
            };

            var results = await influxDb.QueryDataAsync(request);
            return Results.Ok(results);
        })
        .WithName("GetLatestDataV3")
        .WithSummary("Get latest data for a specific measurement");

        group.MapPost("/kafka/test_producer", async (MessageRequest request, IKafkaProducer _kafkaProducer) =>
        {
            if (string.IsNullOrEmpty(request.Topic) || string.IsNullOrEmpty(request.Key) || string.IsNullOrEmpty(request.Value))
            {
                return Results.BadRequest("Topic, Key, and Value must be provided");
            }
            try
            {
                await _kafkaProducer.ProduceAsync(request.Topic, request.Key, request.Value);
                return Results.Ok(new { message = "Message produced successfully" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        return group;
    }
}

public class SimpleQueryRequest
{
    public string Query { get; set; } = string.Empty;
}
