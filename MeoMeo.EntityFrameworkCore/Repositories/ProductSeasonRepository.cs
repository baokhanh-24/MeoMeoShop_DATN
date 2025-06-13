using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProductSeasonRepository : IProductSeasonRepository
    {
        private readonly MeoMeoDbContext _context;
        public ProductSeasonRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public Task<ProductSeason> CreateAsync(ProductSeason entity)
        {
            try
            {
                _context.productSeasons.Add(entity);
                _context.SaveChanges();
                return Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the product season.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var season = await _context.productSeasons.FindAsync(id);
                if (season != null)
                {
                    _context.productSeasons.Remove(season);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the product season.", ex);
            }
        }

        public async Task<List<ProductSeason>> GetAllAsync()
        {
            return await _context.productSeasons.ToListAsync();
        }

        public async Task<ProductSeason> GetByIdAsync(Guid id)
        {
            return await _context.productSeasons.FindAsync(id);
        }

        public async Task<ProductSeason> UpdateAsync(Guid id, ProductSeason entity)
        {
            try
            {
                var existingSeason = await _context.productSeasons.FindAsync(id);
                if (existingSeason == null)
                {
                    throw new Exception("Product season not found.");
                }
                else
                {
                    existingSeason.ProductId = entity.ProductId;
                    existingSeason.SeasonId = entity.SeasonId;
                    _context.productSeasons.Update(existingSeason);
                    _context.SaveChanges();
                    return existingSeason;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the product season.", ex);
            }
        }
    }
}
