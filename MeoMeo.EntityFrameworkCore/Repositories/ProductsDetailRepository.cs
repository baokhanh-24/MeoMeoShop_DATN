using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProductsDetailRepository : BaseRepository<ProductDetail>, IProductsDetailRepository
    {


        public ProductsDetailRepository(MeoMeoDbContext context) : base(context)
        {

        }

        public async Task<ProductDetail> CreateProductDetailAsync(ProductDetail entity)
        {
            var product = await AddAsync(entity);
            return product;
        }

        public async Task<bool> DeleteProductDetailAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ProductDetail>> GetAllProductDetailAsync()
        {
            return await GetAllAsync();
        }

        public async Task<ProductDetail> GetProductDetailByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<ProductDetail> UpdateProductDetailAsync(ProductDetail entity)
        {
            var product = await UpdateAsync(entity);
            return product;
        }
    }
}
