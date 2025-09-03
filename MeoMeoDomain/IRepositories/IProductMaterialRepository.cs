using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductMaterialRepository : IBaseRepository<ProductMaterial>
    {
        Task<IEnumerable<ProductMaterial>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<ProductMaterial>> GetByMaterialIdAsync(Guid materialId);
        Task<bool> DeleteByProductIdAsync(Guid productId);
    }
} 