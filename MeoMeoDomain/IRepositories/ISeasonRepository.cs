using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ISeasonRepository : IBaseRepository<Season>
    {
        Task<IEnumerable<Season>> GetSeasonsAsync();
        public Task<Season> GetSeasonByID(Guid id); 
        public Task<Season> CreateAsync(Season season);
        public Task<Season> UpdateSeason(Season season);
        public Task<bool> DeleteAsync(Guid id);
    }
}
