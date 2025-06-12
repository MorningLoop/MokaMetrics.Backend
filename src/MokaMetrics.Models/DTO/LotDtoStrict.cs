using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokaMetrics.Models.DTO;

public class LotDtoStrict
{
    public int OrderId { get; set; }
    public int TotalQuantity { get; set; }
    public int ManufacturedQuantity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int IndustrialFacilityId { get; set; }

}
