using Microsoft.AspNetCore.Http.HttpResults;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using MokaMetrics.Models.Mappers;

namespace MokaMetrics.API.Endpoints
{
    public static class CustomersEndpoint
    {
        public static IEndpointRouteBuilder MapCustomersEndPoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/customers")
                .WithTags("Customers");

            group.MapGet("/", GetAllCustomersAsync)
               .WithName("GetAllCustomers");
            group.MapGet("/{id:int}", GetCustomerByIdAsync)
                .WithName("GetCustomerById");
            group.MapPost("/", CreateCustomerAsync)
                .WithName("CreateCustomer");
            group.MapPut("/{id:int}", UpdateCustomerAsync)
                .WithName("UpdateCustomer");
            group.MapDelete("/{id:int}", DeleteCustomerAsync)
                .WithName("DeleteCustomer");
            return builder;
        }

        private static async Task<Ok<IEnumerable<Customer>>> GetAllCustomersAsync(IUnitOfWork _uow)
        {
            IEnumerable<Customer> customers = await _uow.Customers.GetAllAsync();
            return TypedResults.Ok(customers ?? Array.Empty<Customer>());
        }
        
        private static async Task<Results<Ok<Customer>, NotFound>> GetCustomerByIdAsync(IUnitOfWork _uow, int id)
        {
            var customer = await _uow.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(customer);
        }
        
        private static async Task<Created<Customer>> CreateCustomerAsync(IUnitOfWork _uow, CustomerCreateDto customerDto)
        {
            var customer = CustomerMapper.MapCreateCustomer(customerDto);
            _uow.Customers.Add(customer);
            await _uow.SaveChangesAsync();
            return TypedResults.Created($"/api/customers/{customer.Id}", customer);
        }
        
        private static async Task<Results<Ok, NotFound>> UpdateCustomerAsync(IUnitOfWork _uow, int id, CustomerDtoStrict customerDto)
        {
            var existingCustomer = await _uow.Customers.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                return TypedResults.NotFound();
            }

            // Update properties
            existingCustomer.Name = customerDto.Name;
            existingCustomer.Email = customerDto.Email;
            existingCustomer.Address = customerDto.Address;
            existingCustomer.Country = customerDto.Country;
            existingCustomer.ZipCode = customerDto.ZipCode;
            existingCustomer.City = customerDto.City;
            existingCustomer.Phone = customerDto.Phone;
            existingCustomer.FiscalId = customerDto.FiscalId;
            existingCustomer.UpdatedAt = DateTime.UtcNow;

            _uow.Customers.Update(existingCustomer);
            await _uow.SaveChangesAsync();

            return TypedResults.Ok();
        }
        
        private static async Task<Results<NoContent, NotFound>> DeleteCustomerAsync(IUnitOfWork _uow, int id)
        {
            var customer = await _uow.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                return TypedResults.NotFound();
            }

            _uow.Customers.Delete(customer);
            await _uow.SaveChangesAsync();

            return TypedResults.NoContent();
        }
    }
}
