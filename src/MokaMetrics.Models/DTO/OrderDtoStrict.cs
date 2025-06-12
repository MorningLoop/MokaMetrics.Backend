namespace MokaMetrics.Models.DTO;

public class OrderDtoStrict
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int QuantityMachines { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? FullfilledDate { get; set; }

}
