using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface ISeasonClientService
    {
        Task<PagingExtensions.PagedResult<SeasonDTO>> GetAllSeasonsAsync(GetListSeasonRequestDTO request);
        Task<SeasonDTO> GetSeasonByIdAsync(Guid id);
        Task<CreateOrUpdateSeasonResponseDTO> CreateSeasonAsync(CreateOrUpdateSeasonDTO dto);
        Task<CreateOrUpdateSeasonResponseDTO> UpdateSeasonAsync(CreateOrUpdateSeasonDTO dto);
        Task<bool> DeleteSeasonAsync(Guid id);
    }
}
