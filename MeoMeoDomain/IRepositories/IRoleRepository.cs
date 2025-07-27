using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IRoleRepository:IBaseRepository<Role>
{
    Task<List<string>> GetNameByRoleIds(IEnumerable<Guid> roleIds);
    Task<Role> GetRoleByName(string roleName);
}