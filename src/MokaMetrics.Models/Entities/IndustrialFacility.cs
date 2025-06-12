namespace MokaMetrics.Models.Entities;

public class IndustrialFacility : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
