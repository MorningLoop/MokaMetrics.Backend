namespace MokaMetrics.Models.DTO;

public class CustomerCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string FiscalId { get; set; } = string.Empty;
}
