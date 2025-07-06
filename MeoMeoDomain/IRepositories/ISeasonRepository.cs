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
        Task<List<Season>> GetSeasonsAsync();
        Task<Season> GetSeasonByIDAsync(Guid id); 
        Task<Season> CreateSeasonAsync(Season season);
        Task<Season> UpdateSeasonAsync(Season season);
        Task<bool> DeleteSeasonAsync(Guid id);
    }
}
