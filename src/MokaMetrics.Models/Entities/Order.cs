namespace MokaMetrics.Models.Entities;

public class Order : Entity
{
    public int CustomerId { get; set; }
    public int QuantityMachines { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? FullfilledDate { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
