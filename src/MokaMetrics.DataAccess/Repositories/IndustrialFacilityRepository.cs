using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Repositories;

public class IndustrialFacilityRepository : Repository<IndustrialFacility>, IIndustrialFacilityRepository
{
    private readonly ApplicationDbContext _context;
    public IndustrialFacilityRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
