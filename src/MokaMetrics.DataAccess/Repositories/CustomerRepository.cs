using Microsoft.EntityFrameworkCore;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    private readonly ApplicationDbContext _context;
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Customer> GetCustomerWithOrdersAsync(int id)
    {
        var customer = await _context.Customers.Include(c => c.Orders)
            .ThenInclude(o => o.Lots)
            .SingleOrDefaultAsync(c => c.Id == id);

        return customer;
    }
}
