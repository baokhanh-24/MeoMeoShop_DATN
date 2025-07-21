using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories;

public class ProductDetailMaterialRepository : BaseRepository<ProductDetailMaterial>, IProductDetailMaterialRepository
{
    public ProductDetailMaterialRepository(MeoMeoDbContext context) : base(context)
    {
    }
}