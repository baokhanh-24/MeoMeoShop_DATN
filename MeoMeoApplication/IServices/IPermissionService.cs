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
        // CRUD operations removed - Permissions are fixed in database
        // Only GET operations are allowed
        Task<List<PermissionDTO>> GetPermissionsByGroupIdAsync(Guid groupId);
        Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(Guid roleId);
    }
}
