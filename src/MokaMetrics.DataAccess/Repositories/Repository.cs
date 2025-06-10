using Microsoft.EntityFrameworkCore;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.Models;
using System.Linq.Expressions;

namespace MokaMetrics.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly IApplicationDbContext _context;

    public Repository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public IQueryable<T> GetQueryable(Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = _context.Set<T>().Where(expression);
        return orderBy != null ? orderBy(query) : query;
    }

    public Task<List<T>> GetListAsync(Expression<Func<T, bool>>? expression, Func<IQueryable<T>,
        IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = expression != null ? _context.Set<T>().Where(expression) : _context.Set<T>();
        return orderBy != null
            ? orderBy(query).ToListAsync(cancellationToken)
            : query.ToListAsync(cancellationToken);
    }

    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _context.Set<T>().ToListAsync(cancellationToken);
    }

    public Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression, string includeProperties, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<T>().AsQueryable();

        query = includeProperties.Split(new char[] { ',' },
            StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty)
            => current.Include(includeProperty));

        return query.SingleOrDefaultAsync(expression);
    }

    public T Add(T entity)
    {
        return _context.Set<T>().Add(entity).Entity;
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
