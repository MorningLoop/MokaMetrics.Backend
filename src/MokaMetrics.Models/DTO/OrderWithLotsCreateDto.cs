namespace MokaMetrics.Models.DTO;

public class OrderWithLotsCreateDto
{
    public int CustomerId { get; set; }
    public int QuantityMachines { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? Deadline { get; set; }
    public List<LotDtoStrict>? Lots { get; set; }
}
