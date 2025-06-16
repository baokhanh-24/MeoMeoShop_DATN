using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
       
       public Task<IEnumerable<Product>> GetAllProductAsync();
        public Task<Product> GetProductByIdAsync(Guid id);
       public Task<Product> AddProductAsync(Product entity);
       public Task<Product> UpdateProductAsync(Product entity);
        public Task<bool> DeleteProductAsync(Guid id);
    }
}
