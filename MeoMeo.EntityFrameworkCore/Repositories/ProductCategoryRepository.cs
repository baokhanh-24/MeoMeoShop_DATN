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
    public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
    {      
        public ProductCategoryRepository(MeoMeoDbContext context) : base(context)
        {
            
        }
        public async Task<IEnumerable<ProductCategory>> GetByProductIdAsync(Guid productId)
        {
            return await Query().Where(x => x.ProductId == productId).ToListAsync();
        }

        public async Task DeleteByProductIdAsync(Guid productId)
        {
            var existingCategories = await GetByProductIdAsync(productId);
            foreach (var category in existingCategories)
            {
                await DeleteAsync(category);
            }
        }
    }
} 