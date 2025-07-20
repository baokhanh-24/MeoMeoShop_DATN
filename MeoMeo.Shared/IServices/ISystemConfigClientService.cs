using MeoMeo.Contract.DTOs.SystemConfig;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface ISystemConfigClientService
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdateSystemConfigDTO>> GetAllSystemConfigsAsync(GetListSystemConfigRequestDTO request);
        Task<CreateOrUpdateSystemConfigDTO> GetSystemConfigByIdAsync(Guid id);
        Task<CreateOrUpdateSystemConfigResponseDTO> CreateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig);
        Task<CreateOrUpdateSystemConfigResponseDTO> UpdateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig);
        Task<bool> DeleteSystemConfigAsync(Guid id);
    }
}
