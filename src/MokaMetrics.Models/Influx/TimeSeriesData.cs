namespace MokaMetrics.Models.Influx;

public class TimeSeriesData
{
    public string Measurement { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
    public Dictionary<string, object> Fields { get; set; } = new();
    public DateTime? Timestamp { get; set; }
}
