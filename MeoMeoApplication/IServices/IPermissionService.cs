using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.IServices
{
    public interface IPermissionService
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
