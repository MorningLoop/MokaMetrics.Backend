using Microsoft.EntityFrameworkCore;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class LotRepository : Repository<Lot>, ILotRepository
{
    private readonly ApplicationDbContext _context;
    public LotRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Lot> GetByCodeAsync(string code)
    {
        return await _context.Lots.FirstOrDefaultAsync(x => x.LotCode == code);
    }
}