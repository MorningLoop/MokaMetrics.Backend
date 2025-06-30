using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using Riok.Mapperly.Abstractions;

namespace MokaMetrics.Models.Mappers;

[Mapper]
public static partial class IndustrialFacilityMapper
{
    public static partial IndustrialFacilityDtoStrict MapToDto(IndustrialFacility industrialFacility);
    public static partial IndustrialFacility MapToEntity(IndustrialFacilityDtoStrict industrialFacilityDto);
}
