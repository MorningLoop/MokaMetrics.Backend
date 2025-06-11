using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly IApplicationDbContext _context;
    public OrderRepository(IApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
