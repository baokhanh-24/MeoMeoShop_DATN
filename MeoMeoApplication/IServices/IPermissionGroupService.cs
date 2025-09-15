using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.IServices
{
    public interface IPermissionGroupService
    {
        Task<List<PermissionGroupDTO>> GetAllPermissionGroupsAsync();
        Task<List<PermissionGroupDTO>> GetPermissionGroupsTreeAsync();
        Task<PermissionGroupDTO> GetPermissionGroupByIdAsync(Guid id);
        // CRUD operations removed - PermissionGroups are fixed in database
        // Only GET operations are allowed
    }
}
