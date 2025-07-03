using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductsDetailRepository : IBaseRepository<ProductDetail>
    {

        public Task<IEnumerable<ProductDetail>> GetAllProductDetailAsync();
        public Task<ProductDetail> GetProductDetailByIdAsync(Guid id);
        public Task<ProductDetail> CreateProductDetailAsync(ProductDetail entity);
        public Task<ProductDetail> UpdateProductDetailAsync(ProductDetail entity);
        public Task<bool> DeleteProductDetailAsync(Guid id);
    }
}
