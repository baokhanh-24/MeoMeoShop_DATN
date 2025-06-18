using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ISeasonServices 
    {
        Task<IEnumerable<Season>> GetAllSeasonsAsync();
        Task<Season> GetSeasonByIdAsync(Guid id);
        Task<Season> CreateSeasonAsync(SeasonDTO dto);
        Task<Season> UpdateSeasonAsync(SeasonDTO dto); 
        Task<bool> DeleteSeasonAsync(Guid id);
    }
}
