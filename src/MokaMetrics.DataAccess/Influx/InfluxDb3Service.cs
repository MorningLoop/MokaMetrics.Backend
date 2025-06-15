using Microsoft.Extensions.Logging;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.DataAccess.Influx.Settings;
using MokaMetrics.Models.Influx;
using InfluxDB3.Client;
using InfluxDB3.Client.Write;

namespace MokaMetrics.DataAccess.Influx;

public class InfluxDb3Service : IInfluxDb3Service, IDisposable
{
    private readonly InfluxDBClient _client;
    private readonly InfluxDb3Settings _settings;
    private readonly ILogger<InfluxDb3Service> _logger;

    public InfluxDb3Service(InfluxDb3Settings settings, ILogger<InfluxDb3Service> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _client = new InfluxDBClient(_settings.Host, _settings.Token, database: _settings.Database);
    }

    public async Task WriteDataAsync(TimeSeriesData data, CancellationToken cancellationToken = default)
    {
        try
        {
            var point = CreatePointData(data);
            await _client.WritePointAsync(point, cancellationToken: cancellationToken);

            _logger.LogDebug("Successfully wrote data point for measurement: {Measurement}", data.Measurement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write data to InfluxDB for measurement: {Measurement}", data.Measurement);
            throw;
        }
    }

    public async Task WriteDataAsync(IEnumerable<TimeSeriesData> data, CancellationToken cancellationToken = default)
    {
        try
        {
            var points = data.Select(CreatePointData).ToArray();
            await _client.WritePointsAsync(points, cancellationToken: cancellationToken);

            _logger.LogDebug("Successfully wrote {Count} data points", data.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write batch data to InfluxDB");
            throw;
        }
    }

    public async Task<IEnumerable<QueryResult>> QueryDataAsync(QueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var sqlQuery = BuildSqlQuery(request);
            var rawResults = await QuerySqlAsync(sqlQuery, request.Fields, cancellationToken);

            return ConvertToQueryResults(rawResults, request.Measurement ?? "unknown");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query data for measurement: {Measurement}", request.Measurement);
            throw;
        }
    }

    public async Task<IEnumerable<Dictionary<string, object>>> QuerySqlAsync(string sqlQuery, IList<string> selectFields, CancellationToken cancellationToken = default)
    {
        try
        {
            List<Dictionary<string, object>> results = new();

            selectFields = selectFields?.Count() > 0 ? selectFields : new string[] { "time", "location", "machine", "value" };

            await foreach (var row in _client.Query(query: sqlQuery))
            {
                var dict = new Dictionary<string, object>();

                for (int i = 0; i < row.Length; i++)
                {
                    dict[selectFields[i]] = row[i];
                }

                results.Add(dict);
            }

            _logger.LogDebug("Successfully executed SQL query and retrieved {Count} rows", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute SQL query: {Query}", sqlQuery);
            throw;
        }
    }

    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple health check by querying system information
            var healthQuery = "SELECT 1 as health_check LIMIT 1";
            var results = await QuerySqlAsync(healthQuery, [], cancellationToken);
            return results.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InfluxDB health check failed");
            return false;
        }
    }

    private PointData CreatePointData(TimeSeriesData data)
    {
        var point = PointData.Measurement(data.Measurement);

        // Set timestamp if provided
        if (data.Timestamp.HasValue)
        {
            point = point.SetTimestamp(data.Timestamp.Value);
        }

        // Add tags
        foreach (var tag in data.Tags)
        {
            point = point.SetTag(tag.Key, tag.Value?.ToString() ?? string.Empty);
        }

        // Add fields
        foreach (var field in data.Fields)
        {
            point = AddFieldToPoint(point, field.Key, field.Value);
        }

        return point;
    }

    private static PointData AddFieldToPoint(PointData point, string key, object? value)
    {
        return value switch
        {
            string s => point.SetField(key, s),
            int i => point.SetField(key, i),
            long l => point.SetField(key, l),
            double d => point.SetField(key, d),
            float f => point.SetField(key, f),
            bool b => point.SetField(key, b),
            decimal dec => point.SetField(key, (double)dec),
            DateTime dt => point.SetField(key, dt.ToString("O")),
            _ => point.SetField(key, value?.ToString() ?? string.Empty)
        };
    }

    private string BuildSqlQuery(QueryRequest request)
    {
        var selectClause = "SELECT time, location, machine, value";

        // Specify fields if provided
        if (request.Fields?.Any() == true)
        {
            var fieldList = string.Join(", ", request.Fields);
            selectClause = $"SELECT time, {fieldList}";

            // Add tags to select if they're used in filtering
            if (request.Tags?.Any() == true)
            {
                var tagList = string.Join(", ", request.Tags.Keys);
                selectClause = $"SELECT time, {tagList}, {fieldList}";
            }
        }

        var query = selectClause;

        // FROM clause with measurement
        if (!string.IsNullOrEmpty(request.Measurement))
        {
            query += $" FROM {request.Measurement}";
        }
        else
        {
            query += $" FROM {_settings.Database}";
        }

        var whereConditions = new List<string>();

        // Time range conditions
        if (request.StartTime.HasValue)
        {
            whereConditions.Add($"time >= '{request.StartTime.Value:yyyy-MM-ddTHH:mm:ssZ}'");
        }

        if (request.EndTime.HasValue)
        {
            whereConditions.Add($"time <= '{request.EndTime.Value:yyyy-MM-ddTHH:mm:ssZ}'");
        }

        // Tag filters
        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                whereConditions.Add($"\"{tag.Key}\" = '{EscapeSqlString(tag.Value)}'");
            }
        }

        // Add WHERE clause if there are conditions
        if (whereConditions.Any())
        {
            query += " WHERE " + string.Join(" AND ", whereConditions);
        }

        // Aggregation (simplified - InfluxDB 3.0 uses standard SQL aggregation)
        if (!string.IsNullOrEmpty(request.AggregateFunction) && request.WindowDuration.HasValue)
        {
            var interval = FormatSqlInterval(request.WindowDuration.Value);
            query = query.Replace("SELECT *", $"SELECT time_bucket('{interval}', time) as time_bucket, {request.AggregateFunction}(*) as value");
            query += " GROUP BY time_bucket";
        }

        // ORDER BY time (most recent first)
        query += " ORDER BY time DESC";

        // Limit
        if (request.Limit.HasValue)
        {
            query += $" LIMIT {request.Limit.Value}";
        }

        return query;
    }

    private static string FormatSqlInterval(TimeSpan duration)
    {
        if (duration.TotalDays >= 1)
            return $"{(int)duration.TotalDays} days";
        if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours} hours";
        if (duration.TotalMinutes >= 1)
            return $"{(int)duration.TotalMinutes} minutes";
        return $"{(int)duration.TotalSeconds} seconds";
    }

    private static string EscapeSqlString(string value)
    {
        return value.Replace("'", "''");
    }

    private static string EscapeSqlValue(object value)
    {
        return value switch
        {
            string s => $"'{EscapeSqlString(s)}'",
            DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
            bool b => b.ToString().ToLower(),
            null => "NULL",
            _ => value.ToString() ?? "NULL"
        };
    }

    private static IEnumerable<QueryResult> ConvertToQueryResults(IEnumerable<Dictionary<string, object>> rawResults, string measurement)
    {
        return rawResults.Select(row =>
        {
            var result = new QueryResult
            {
                Measurement = measurement,
                Tags = new Dictionary<string, object>(),
                Fields = new Dictionary<string, object>()
            };

            foreach (var (key, value) in row)
            {
                if (key.Equals("time", StringComparison.OrdinalIgnoreCase))
                {
                    if (long.TryParse(value?.ToString(), out var nanoseconds))
                    {
                        DateTime epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        result.Timestamp = epochTime.AddTicks(nanoseconds / 100);
                    }


                    //if (DateTime.TryParse(value?.ToString(), out var timestamp))
                    //{
                    //    result.Timestamp = timestamp;
                    //}
                }
                else if (IsTagColumn(key))
                {
                    result.Tags[key] = value ?? string.Empty;
                }
                else
                {
                    result.Fields[key] = value ?? string.Empty;
                }
            }

            return result;
        });
    }

    private static bool IsTagColumn(string columnName)
    {
        // In InfluxDB 3.0, tags are typically string columns that are not time or numeric fields
        // This is a simplified heuristic - you might want to adjust based on your schema
        return !columnName.Equals("time", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}