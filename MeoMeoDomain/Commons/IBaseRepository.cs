using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Commons
{
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

    }
}
