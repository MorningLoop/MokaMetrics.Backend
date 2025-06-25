namespace MokaMetrics.Models.Influx;

public class QueryRequest
{
    public string? Measurement { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? AggregateFunction { get; set; } // mean, sum, count, etc.
    public TimeSpan? WindowDuration { get; set; }
    public int? Limit { get; set; }
    public List<string>? Fields { get; set; } // Specific fields to query
}
