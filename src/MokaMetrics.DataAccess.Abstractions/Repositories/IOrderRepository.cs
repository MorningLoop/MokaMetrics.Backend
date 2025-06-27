using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetOrderWithLotsAsync(int orderId);
}
