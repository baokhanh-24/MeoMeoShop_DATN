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
    public class ProductSeasonRepository : BaseRepository<ProductSeason>, IProductSeasonRepository
    {
        
        public ProductSeasonRepository(MeoMeoDbContext context) : base(context)
        {
        }


        public Task<ProductSeason> CreateProductSeasonAsync(ProductSeason productSeason)
        {
            var newProductSeason = new ProductSeason
            {
                ProductId = productSeason.ProductId,
                SeasonId = productSeason.SeasonId,
            };
            return AddAsync(newProductSeason);
        }

        public async Task<bool> DeleteProductSeasonAsync(Guid ProductId, Guid SeasonId)
        {
            var phat = await _dbSet.FirstOrDefaultAsync(ps => ps.ProductId == ProductId && ps.SeasonId == SeasonId);
            if (phat != null)
            {
                _dbSet.Remove(phat);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ProductSeason>> GeProductSeasontAllAsync()
        {
            var phat = await GetAllAsync();
            return phat;
        }

        public async Task<ProductSeason> GetProductSeasonByIdAsync(Guid ProductId, Guid SeasonId)
        {
            return await _dbSet.FirstOrDefaultAsync(ps => ps.ProductId == ProductId && ps.SeasonId == SeasonId);
        }

        public void RemoveProductSeason(ProductSeason productSeason)
        {
            _dbSet.Remove(productSeason);
        }

        public async Task<ProductSeason> UpdateProductSeasonAsync(ProductSeason productSeason)
        {
            var phat = await _dbSet.FirstOrDefaultAsync(ps => ps.ProductId == productSeason.ProductId && ps.SeasonId == productSeason.SeasonId);
            if (phat == null)
            {
                return null;
            }
            phat.ProductId = productSeason.ProductId;
            phat.SeasonId = productSeason.SeasonId;
            return await UpdateAsync(phat);
        }
    }
}
