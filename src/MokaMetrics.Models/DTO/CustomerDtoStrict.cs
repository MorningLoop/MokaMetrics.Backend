using MokaMetrics.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokaMetrics.Models.DTO;

public class CustomerDtoStrict
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string FiscalId { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set;  } = new List<Order>();
}
