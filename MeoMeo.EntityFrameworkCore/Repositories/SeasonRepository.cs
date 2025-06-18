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
    public class SeasonRepository : BaseRepository<Season>, ISeasonRepository
    {
        public SeasonRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Season> CreateAsync(Season season)
        {
            try
            {
                await AddAsync(season);
                return season;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while creating the season.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var phat = await GetByIdAsync(id);
            if (phat == null)
            {
                return false;
            }
            await base.DeleteAsync(id);
            return true;
        }

        public async Task<Season> GetSeasonByID(Guid id)
        {
            var season = await GetByIdAsync(id);
            return season;
        }

        public async Task<IEnumerable<Season>> GetSeasonsAsync()
        {
            return await GetAllAsync(); 
        }

        public async Task<Season> UpdateSeason(Season season)
        {
            var phat = await GetByIdAsync(season.Id);
            if (phat == null)
            {
                throw new Exception("Season not found.");
            }
            phat.Name = season.Name;
            phat.Description = season.Description;
            phat.Status = season.Status;
            await UpdateAsync(phat);
            return phat;
        }
    }
}
