using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductDetaillSizeRepository : IBaseRepository<ProductDetailSize>
    {
        public Task<IEnumerable<ProductDetailSize>> GetAllProductDetaillSize();
        public Task<ProductDetailSize> GetProductDetaillSizeById(Guid id);
        public Task<ProductDetailSize> Create(ProductDetailSize productDetailSize);
        public Task<ProductDetailSize> Update(ProductDetailSize productDetailSize);
        public Task<bool> Delete(Guid id);
    }
}
