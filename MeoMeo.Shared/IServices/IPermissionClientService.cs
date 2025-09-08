using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.IServices
{
    public interface IPermissionClientService
    {
        Task<List<PermissionDTO>> GetAllPermissionsAsync();
        Task<PermissionDTO> GetPermissionByIdAsync(Guid id);
        Task<BaseResponse> CreatePermissionAsync(CreateOrUpdatePermissionDTO dto);
        Task<BaseResponse> UpdatePermissionAsync(CreateOrUpdatePermissionDTO dto);
        Task<BaseResponse> DeletePermissionAsync(Guid id);
        Task<List<PermissionDTO>> GetPermissionsByGroupIdAsync(Guid groupId);
        Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(Guid roleId);
    }
}
