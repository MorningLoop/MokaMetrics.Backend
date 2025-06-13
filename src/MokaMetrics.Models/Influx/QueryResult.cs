namespace MokaMetrics.Models.Influx;

public class QueryResult
{
    public string Measurement { get; set; } = string.Empty;
    public Dictionary<string, object> Tags { get; set; } = new();
    public Dictionary<string, object> Fields { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

