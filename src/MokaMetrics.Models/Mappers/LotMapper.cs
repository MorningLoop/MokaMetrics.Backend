using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using Riok.Mapperly.Abstractions;

namespace MokaMetrics.Models.Mappers;

[Mapper]
public static partial class LotMapper
{
    public static partial Lot MapToEntity(LotDtoStrict orderDto);
}
