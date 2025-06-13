using MeoMeo.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ISeasonServices
    {
        Task<List<SeasonDTO>> GetAllSeasonsAsync();
        Task<SeasonDTO> GetSeasonByIdAsync(Guid id);
        Task<SeasonDTO> CreateSeasonAsync(SeasonDTO dto);
        Task<SeasonDTO> UpdateSeasonAsync(Guid id, SeasonDTO dto);
        Task<bool> DeleteSeasonAsync(Guid id);
    }
}
