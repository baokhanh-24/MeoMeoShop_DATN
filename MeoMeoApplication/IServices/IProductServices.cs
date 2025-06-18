using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductServices
    {
       public Task<Product> GetProductByIdAsync(Guid id);
       public Task<IEnumerable<Product>> GetAllAsync();
        public Task<Product> CreateProductAsync(CreateOrUpdateProductDTO product);
        public Task UpdateAsync(Product model);
        public Task DeleteAsync(Guid id);
    }
}
