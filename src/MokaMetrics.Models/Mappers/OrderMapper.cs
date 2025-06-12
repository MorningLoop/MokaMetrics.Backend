using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using Riok.Mapperly.Abstractions;

namespace MokaMetrics.Models.Mappers;

[Mapper]
public static partial class OrderMapper 
{
    public static partial OrderDtoStrict MapToDto(Order order);
    public static partial Order MapToEntity(OrderDtoStrict orderDto);
    public static partial Order MapCreateOrder(OrderWithLotsCreateDto order);
}
