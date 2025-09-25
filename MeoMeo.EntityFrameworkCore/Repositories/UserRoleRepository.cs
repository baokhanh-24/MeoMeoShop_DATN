using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserRole>> GetRolesByUserId(Guid userId)
        {
            var userRoles = await _context.userRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            return userRoles;
        }

        public async Task<UserRole> AddUserRole(UserRole userRole)
        {
            await _context.userRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public void RemoveUserRole(UserRole userRole)
        {
            _context.userRoles.Remove(userRole);
        }
    }
}