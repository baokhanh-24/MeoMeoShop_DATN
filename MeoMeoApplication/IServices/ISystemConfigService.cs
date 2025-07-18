using MeoMeo.Contract.DTOs.SystemConfig;
using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ISystemConfigService
    {
        Task<PagingExtensions.PagedResult<SystemConfigDTO>> GetAllSystemConfigAsync(GetListSystemConfigRequestDTO request);
        Task<CreateOrUpdateSystemConfigResponseDTO> GetSystemConfigByIdAsync(Guid id);
        Task<CreateOrUpdateSystemConfigResponseDTO> CreateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig);
        Task<CreateOrUpdateSystemConfigResponseDTO> UpdateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig);
        Task<bool> DeleteSystemConfigAsync(Guid id);
    }
}
