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
    public class SeasonRepository : ISeasonRepository
    {
        private readonly MeoMeoDbContext _context;
        public SeasonRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task<Season> CreateSeasonAsync(Season season)
        {
            try
            {
                _context.seasons.Add(season);
                await _context.SaveChangesAsync();
                return season;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the season.", ex);
            }
        }

        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            try
            {
                var x = await _context.seasons.FindAsync(id);
                if (x != null)
                {
                    _context.seasons.Remove(x);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the season.", ex);
            }
        }

        public async Task<List<Season>> GetAllSeasonsAsync()
        {
            return await _context.seasons.ToListAsync();
        }

        public async Task<Season> GetSeasonByIdAsync(Guid id)
        {
            return await _context.seasons.FindAsync(id);
        }

        public async Task<Season> UpdateSeasonAsync(Guid id, Season season)
        {
            try
            {
                var x = await _context.seasons.FindAsync(id);
                if (x == null)
                {
                    throw new Exception("Season not found.");
                }
                x.Name = season.Name;
                x.Description = season.Description;
                _context.seasons.Update(x);
                await _context.SaveChangesAsync();
                return x;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the season.", ex);
            }
        }
    }
}
