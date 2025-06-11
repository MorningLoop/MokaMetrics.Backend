using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Contexts;

namespace MokaMetrics.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly IApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    public UnitOfWork(IApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    // add custom repositories interfaces using this format:
    // private readonly IRepoName? _repoName;
    // public IRepoName Name => _repoName ??= _serviceProvider.GetRequiredService<IRepoName>();

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        // Save changes in the context
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
