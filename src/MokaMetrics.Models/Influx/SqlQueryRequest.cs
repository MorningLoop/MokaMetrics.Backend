namespace MokaMetrics.Models.Influx;

public class SqlQueryRequest
{
    public string Query { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; }
}
