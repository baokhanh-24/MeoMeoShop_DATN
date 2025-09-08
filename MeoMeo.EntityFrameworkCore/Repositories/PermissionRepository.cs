using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class PermissionRepository:BaseRepository<Permission>, IPermissionRepository
    {
        private readonly MeoMeoDbContext _context;


        public PermissionRepository(MeoMeoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetPermissionByUserId(Guid UserId)
        {
            // Get role permissions for the given user through UserRole
            var userRoles = await _context.userRoles
                .Where(ur => ur.UserId == UserId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var rolePermissions = await _context.rolePermissions
                .Where(rp => userRoles.Contains(rp.RoleId))
                .ToListAsync();

            return rolePermissions;
        }

        public async Task<bool> AddPermissionToRole(Guid RoleId, List<RolePermission> input)
        {
            try
            {
                await _context.rolePermissions.AddRangeAsync(input);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 