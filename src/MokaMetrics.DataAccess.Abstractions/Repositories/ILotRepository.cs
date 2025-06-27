using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Abstractions.Repositories;

public interface ILotRepository : IRepository<Lot>
{
    Task<Lot> GetByCodeAsync(string code);
}
