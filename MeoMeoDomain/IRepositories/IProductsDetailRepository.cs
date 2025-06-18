using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public  interface IProductsDetailRepository : IBaseRepository<ProductDetail>
    {
       
       public Task<IEnumerable<ProductDetail>> GetAllProductAsync();
       public Task<ProductDetail> GetByProductIdAsync(Guid id);
       public Task<ProductDetail> AddProductAsync(ProductDetail entity);
        public Task<ProductDetail> UpdateProductAsync(ProductDetail entity);
       public Task<bool> DeleteProductAsync(Guid id);
    }
}
