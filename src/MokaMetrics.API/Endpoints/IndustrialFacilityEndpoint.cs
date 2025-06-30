using Microsoft.AspNetCore.Http.HttpResults;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using MokaMetrics.Models.Mappers;

namespace MokaMetrics.API.Endpoints;

public static class IndustrialFacilityEndpoint
{
    public static IEndpointRouteBuilder MapIndustrialFacilityEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/industrialFacilities")
               .WithTags("IndustrialFacilities");

        group.MapGet("/", GetIndustrialFacilities)
           .WithName("GetAllIndustrialFacilities");
        
        return builder;
    }
    private static async Task<Ok<IEnumerable<IndustrialFacilityDtoStrict>>> GetIndustrialFacilities(IUnitOfWork _uow)
    {
        var ifs = await _uow.IndustrialFacilities.GetAllAsync();
        var ifsDto = ifs.Select(IndustrialFacilityMapper.MapToDto);
        return TypedResults.Ok(ifsDto ?? Array.Empty<IndustrialFacilityDtoStrict>());
    }
}
