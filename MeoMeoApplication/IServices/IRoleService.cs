using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.IServices
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(Guid id);
        Task<BaseResponse> CreateRoleAsync(CreateOrUpdateRoleDTO dto);
        Task<BaseResponse> UpdateRoleAsync(CreateOrUpdateRoleDTO dto);
        Task<BaseResponse> DeleteRoleAsync(Guid id);
        Task<BaseResponse> AssignPermissionsToRoleAsync(AssignPermissionsToRoleDTO dto);
        Task<List<PermissionDTO>> GetRolePermissionsAsync(Guid roleId);
        Task<List<UserRoleDTO>> GetRoleUsersAsync(Guid roleId);
    }
}
