using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories;

public class ProductMaterialRepository : BaseRepository<ProductMaterial>, IProductMaterialRepository
{
    public ProductMaterialRepository(MeoMeoDbContext context) : base(context)
    {
    }

    public Task<IEnumerable<ProductMaterial>> GetByProductIdAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductMaterial>> GetByMaterialIdAsync(Guid materialId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByProductIdAsync(Guid productId)
    {
        throw new NotImplementedException();
    }
}