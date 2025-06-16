using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(MeoMeoDbContext context) : base(context)
        {
          
        }

        public async Task<Product> AddProductAsync(Product entity)
        {
            var product = await AddAsync(entity);
            return product;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;

        }

        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Product> UpdateProductAsync(Product entity)
        {
            var product = await UpdateAsync(entity);
            return product;
        }
    }
}
