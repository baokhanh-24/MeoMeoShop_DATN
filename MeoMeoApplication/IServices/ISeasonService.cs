using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ISeasonService 
    {
        Task<PagingExtensions.PagedResult<SeasonDTO>> GetAllSeasonsAsync(GetListSeasonRequestDTO request);
        Task<SeasonDTO> GetSeasonByIdAsync(Guid id);
        Task<CreateOrUpdateSeasonResponseDTO> CreateSeasonAsync(CreateOrUpdateSeasonDTO dto);
        Task<CreateOrUpdateSeasonResponseDTO> UpdateSeasonAsync(CreateOrUpdateSeasonDTO dto); 
        Task<bool> DeleteSeasonAsync(Guid id);
    }
}
