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

       public async Task<Product> GetProductAsync(Guid id)
        {
             var product = await GetByIdAsync(id);
            return product;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.products.ToListAsync();
        }
        public async Task<Product?> GetByIdAsnyc (Guid id)
        {
            return await _context.products.FindAsync(id);
        }
        public async Task AddAsync(Product entity)
        {
            await _context.products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product entity)
        {
            _context.products.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                _context.products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

    }
}
