using System.Linq.Expressions;
using MeoMeo.Domain.Commons;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(object id);
    Task SaveChangesAsync();
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> Query();

    Task<PagingExtensions.PagedResult<TEntity>> GetPagedAsync(
        IQueryable<TEntity> query,
        int pageIndex,
        int pageSize);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
}