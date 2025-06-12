
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class MachineActivityStatusRepository : Repository<MachineActivityStatus>, IMachineActivityStatusRepository
{
    private readonly ApplicationDbContext _context;
    public MachineActivityStatusRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
