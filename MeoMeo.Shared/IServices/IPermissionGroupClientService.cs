using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.IServices
{
    public interface IPermissionGroupClientService
    {
        Task<List<PermissionGroupDTO>> GetAllPermissionGroupsAsync();
        Task<List<PermissionGroupDTO>> GetPermissionGroupsTreeAsync();
        Task<PermissionGroupDTO> GetPermissionGroupByIdAsync(Guid id);
        Task<BaseResponse> CreatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto);
        Task<BaseResponse> UpdatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto);
        Task<BaseResponse> DeletePermissionGroupAsync(Guid id);
    }
}
