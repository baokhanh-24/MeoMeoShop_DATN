using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ISeasonRepository
    {
        Task<List<Season>> GetAllSeasonsAsync();
        Task<Season> GetSeasonByIdAsync(Guid id);
        Task<Season> CreateSeasonAsync(Season season);
        Task<Season> UpdateSeasonAsync(Guid id, Season season);
        Task<bool> DeleteSeasonAsync(Guid id);
    }
}
