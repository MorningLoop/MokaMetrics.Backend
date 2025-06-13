using MokaMetrics.Models.Influx;

namespace MokaMetrics.DataAccess.Abstractions.Influx;

public interface IInfluxDb3Service
{
    Task WriteDataAsync(TimeSeriesData data, CancellationToken cancellationToken = default);
    Task WriteDataAsync(IEnumerable<TimeSeriesData> data, CancellationToken cancellationToken = default);
    Task<IEnumerable<QueryResult>> QueryDataAsync(QueryRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Dictionary<string, object>>> QuerySqlAsync(string sqlQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<Dictionary<string, object>>> QuerySqlAsync(SqlQueryRequest request, CancellationToken cancellationToken = default);
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}
