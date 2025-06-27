using Microsoft.EntityFrameworkCore;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _context;
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Order> GetOrderWithLotsAsync(int orderId)
    {
        return await _context.Orders.Include(o => o.Lots).FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
