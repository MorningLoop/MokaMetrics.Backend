using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using Riok.Mapperly.Abstractions;

namespace MokaMetrics.Models.Mappers;

[Mapper]
public static partial class CustomerMapper
{
    public static partial CustomerDtoStrict MapToDto(Customer customer);
}
