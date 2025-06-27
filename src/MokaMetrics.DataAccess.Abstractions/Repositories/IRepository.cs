using MokaMetrics.Models;
using System.Linq.Expressions;

namespace MokaMetrics.DataAccess.Abstractions.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<T> GetQueryable(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>>? expression, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression, string includeProperties, CancellationToken cancellationToken = default);
    T Add(T entity);
    abstract void Update(T entity);
    abstract void UpdateRange(IEnumerable<T> entities);
    void Delete(T entity);
    void AddRange(IEnumerable<T> values);
}
