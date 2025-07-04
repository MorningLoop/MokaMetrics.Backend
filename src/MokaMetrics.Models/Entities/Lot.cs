﻿namespace MokaMetrics.Models.Entities;

public class Lot : Entity
{
    public int OrderId { get; set; }
    public string LotCode { get; set; } = $"LOT-{DateTime.UtcNow.ToString("yyyy-mm-dd-mm-ss")}";
    public int TotalQuantity { get; set; }
    public int ManufacturedQuantity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int IndustrialFacilityId { get; set; }

    public virtual Order Order { get; set; }
    public virtual IndustrialFacility IndustrialFacility { get; set; }
}
