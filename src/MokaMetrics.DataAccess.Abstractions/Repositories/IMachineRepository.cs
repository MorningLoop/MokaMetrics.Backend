using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Abstractions.Repositories;

public interface IMachineRepository : IRepository<Machine>
{
    public Task<Machine> GetByCodeAsync(string code);
}
