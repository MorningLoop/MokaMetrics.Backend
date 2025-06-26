namespace MokaMetrics.Models.DTO;

public class OrderWithLotsCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int QuantityMachines { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? FullfilledDate { get; set; }
    public List<LotDtoStrict>? Lots { get; set; }
}
