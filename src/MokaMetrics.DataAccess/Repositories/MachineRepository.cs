using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class MachineRepository : Repository<Machine>, IMachineRepository
{
    private readonly ApplicationDbContext _context;
    public MachineRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Machine> GetByCodeAsync(string code)
    {
        return _context.Machines.FirstOrDefault(m => m.Code == code) ?? throw new KeyNotFoundException($"Machine with code {code} not found.");
    }
}
