namespace MokaMetrics.Models.Entities;

public class Machine
{
    public string Model { get; set; }
    public int Status { get; set; }
    public int IndustrialFacilityId { get; set; }
    
    public virtual IndustrialFacility IndustrialFacility { get; set; }
}
