using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IPermissionRepository: IBaseRepository<Permission>
{
    Task<IEnumerable<RolePermission>> GetPermissionByUserId(Guid UserId);
    Task<bool> AddPermissionToRole(Guid RoleId,List<RolePermission> input);
}