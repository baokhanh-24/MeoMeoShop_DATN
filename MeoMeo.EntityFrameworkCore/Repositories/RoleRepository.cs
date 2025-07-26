using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<List<string>> GetNameByRoleIds(IEnumerable<Guid> roleIds)
        {
            var roles = await _context.roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            return roles;
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            return await _context.roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
} 