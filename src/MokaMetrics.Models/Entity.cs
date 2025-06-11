using System.ComponentModel.DataAnnotations;

namespace MokaMetrics.Models;

public class Entity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
