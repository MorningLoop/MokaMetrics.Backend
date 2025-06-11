namespace MokaMetrics.DataAccess.Abstractions;

public interface IUnitOfWork
{
    // add here other repositories as needed
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
