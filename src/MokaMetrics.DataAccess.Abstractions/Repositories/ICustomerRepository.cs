using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Abstractions.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetCustomerWithOrdersAsync(int id);
}
