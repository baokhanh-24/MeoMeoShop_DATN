using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IRolePermissionRepository : IBaseRepository<RolePermission>
{
    Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
    Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId);
    Task<bool> ExistsAsync(Guid roleId, Guid permissionId);
    Task DeleteByRoleIdAsync(Guid roleId);
    Task DeleteByPermissionIdAsync(Guid permissionId);
}
