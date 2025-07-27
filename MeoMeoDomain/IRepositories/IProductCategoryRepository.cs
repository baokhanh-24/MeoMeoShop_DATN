using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IProductCategoryRepository:IBaseRepository<ProductCategory>
{
    public Task<IEnumerable<ProductCategory>> GetByProductIdAsync(Guid productId);

    public Task DeleteByProductIdAsync(Guid productId);

}