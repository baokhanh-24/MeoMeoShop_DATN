using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    Task<IEnumerable<UserRole>> GetRolesByUserId(Guid userId);
    Task<UserRole> AddUserRole(UserRole userRole);
    void RemoveUserRole(UserRole userRole);
}