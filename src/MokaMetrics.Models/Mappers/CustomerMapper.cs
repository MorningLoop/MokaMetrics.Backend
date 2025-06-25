using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using Riok.Mapperly.Abstractions;

namespace MokaMetrics.Models.Mappers;

[Mapper]
public static partial class CustomerMapper
{
    [MapperIgnoreSource(nameof(Customer.Orders))]
    [MapperIgnoreSource(nameof(Customer.CreatedAt))]
    [MapperIgnoreSource(nameof(Customer.UpdatedAt))]
    public static partial CustomerDtoStrict MapToDto(Customer customer);
    
    [MapperIgnoreTarget(nameof(Customer.Orders))]
    [MapperIgnoreTarget(nameof(Customer.CreatedAt))]
    [MapperIgnoreTarget(nameof(Customer.UpdatedAt))]
    public static partial Customer MapToEntity(CustomerDtoStrict customerDto);
    
    [MapperIgnoreTarget(nameof(Customer.Orders))]
    [MapperIgnoreTarget(nameof(Customer.Id))]
    [MapperIgnoreTarget(nameof(Customer.CreatedAt))]
    [MapperIgnoreTarget(nameof(Customer.UpdatedAt))]
    public static partial Customer MapCreateCustomer(CustomerCreateDto customerDto);
}
