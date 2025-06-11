using MokaMetrics.Models.Helpers;

namespace MokaMetrics.Models.Entities;

public class MachineActivityStatus
{
    public int MachineId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int Status { get; set; }
    public string? ErrorMessage { get; set; }

    public virtual Machine Machine { get; set; }
}
