using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.Models.Influx;

namespace MokaMetrics.API.Endpoints;

public static class TestEndpoints
{
    public static IEndpointRouteBuilder MapInfluxTestEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/iot")
            .WithTags("Time Series Data (InfluxDB 3.0)");

        // Write single data point
        group.MapPost("/", async (TimeSeriesData data, IInfluxDb3Service influxDb) =>
        {
            await influxDb.WriteDataAsync(data);
            return Results.Ok(new { message = "Data written successfully" });
        })
        .WithName("WriteTimeSeriesDataV3")
        .WithSummary("Write a single time series data point to InfluxDB 3.0");

        // Write multiple data points
        group.MapPost("/batch", async (IEnumerable<TimeSeriesData> data, IInfluxDb3Service influxDb) =>
        {
            await influxDb.WriteDataAsync(data);
            return Results.Ok(new { message = $"{data.Count()} data points written successfully" });
        })
        .WithName("WriteBatchTimeSeriesDataV3")
        .WithSummary("Write multiple time series data points to InfluxDB 3.0");

        // Query data with structured request
        group.MapPost("/query", async (QueryRequest request, IInfluxDb3Service influxDb) =>
        {
            var results = await influxDb.QueryDataAsync(request);
            return Results.Ok(results);
        })
        .WithName("QueryTimeSeriesDataV3")
        .WithSummary("Query time series data using structured request");

        // Raw SQL query
        group.MapPost("/query/sql", async (SqlQueryRequest request, IInfluxDb3Service influxDb) =>
        {
            var results = await influxDb.QuerySqlAsync(request);
            return Results.Ok(results);
        })
        .WithName("SqlQueryV3")
        .WithSummary("Execute raw SQL query against InfluxDB 3.0");

        // Simple SQL query
        group.MapPost("/query/sql/simple", async (SimpleQueryRequest request, IInfluxDb3Service influxDb) =>
        {
            var results = await influxDb.QuerySqlAsync(request.Query);
            return Results.Ok(results);
        })
        .WithName("SimpleSqlQueryV3")
        .WithSummary("Execute simple SQL query");

        // Health check
        group.MapGet("/health", async (IInfluxDb3Service influxDb) =>
        {
            Console.WriteLine("entered /health endpoint");
            var isHealthy = await influxDb.HealthCheckAsync();
            return isHealthy ? Results.Ok(new { status = "healthy" }) : Results.Problem("InfluxDB 3.0 is unhealthy");
        })
        .WithName("InfluxDb3HealthCheck")
        .WithSummary("Check InfluxDB 3.0 health");

        // Get latest data for a measurement
        group.MapGet("/latest/{measurement}", async (string measurement, IInfluxDb3Service influxDb, int limit = 10) =>
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

        return group;
    }
}

public class SimpleQueryRequest
{
    public string Query { get; set; } = string.Empty;
}
