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

        public async Task<Season> CreateSeasonAsync(Season season)
        {
           var seasonCreate = await AddAsync(season);
            return seasonCreate;    
        }

        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<Season> GetSeasonByIDAsync(Guid id)
        {
            var season = await GetByIdAsync(id);
            return season;
        }

        public async Task<List<Season>> GetSeasonsAsync()
        {
            var seasonAll = await GetAllAsync();
            return seasonAll.ToList(); 
        }

        public async Task<Season> UpdateSeasonAsync(Season season)
        {
           var seasonUpdate = await UpdateAsync(season);
            return seasonUpdate;
        }
    }
}
