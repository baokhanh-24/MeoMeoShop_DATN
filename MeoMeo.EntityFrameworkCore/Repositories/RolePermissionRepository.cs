using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            return await _dbSet
                .Include(rp => rp.Permission)
                .ThenInclude(p => p.PermissionGroup)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId)
        {
            return await _dbSet
                .Include(rp => rp.Role)
                .Where(rp => rp.PermissionId == permissionId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId)
        {
            return await _dbSet
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public async Task DeleteByRoleIdAsync(Guid roleId)
        {
            var rolePermissions = await _dbSet
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            if (rolePermissions.Any())
            {
                _dbSet.RemoveRange(rolePermissions);
                await SaveChangesAsync();
            }
        }

        public async Task DeleteByPermissionIdAsync(Guid permissionId)
        {
            var rolePermissions = await _dbSet
                .Where(rp => rp.PermissionId == permissionId)
                .ToListAsync();

            if (rolePermissions.Any())
            {
                _dbSet.RemoveRange(rolePermissions);
                await SaveChangesAsync();
            }
        }
    }
}
