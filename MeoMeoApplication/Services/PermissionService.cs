using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionGroupRepository _permissionGroupRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IMapper _mapper;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IPermissionGroupRepository permissionGroupRepository,
            IRolePermissionRepository rolePermissionRepository,
            IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _permissionGroupRepository = permissionGroupRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _mapper = mapper;
        }

        public async Task<List<PermissionDTO>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.Query()
                .Include(p => p.PermissionGroup)
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Function = p.Function,
                    PermissionGroupId = p.PermissionGroupId,
                    PermissionGroupName = p.PermissionGroup.Name,
                    Command = p.Command,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();

            return permissions;
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(Guid id)
        {
            var permission = await _permissionRepository.Query()
                .Include(p => p.PermissionGroup)
                .Where(p => p.Id == id)
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Function = p.Function,
                    PermissionGroupId = p.PermissionGroupId,
                    PermissionGroupName = p.PermissionGroup.Name,
                    Command = p.Command,
                    Name = p.Name,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();

            return permission ?? new PermissionDTO();
        }

        // CRUD operations removed - Permissions are fixed in database
        // Only GET operations are allowed

        public async Task<List<PermissionDTO>> GetPermissionsByGroupIdAsync(Guid groupId)
        {
            var permissions = await _permissionRepository.Query()
                .Include(p => p.PermissionGroup)
                .Where(p => p.PermissionGroupId == groupId)
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Function = p.Function,
                    PermissionGroupId = p.PermissionGroupId,
                    PermissionGroupName = p.PermissionGroup.Name,
                    Command = p.Command,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();

            return permissions;
        }

        public async Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId);

            return rolePermissions.Select(rp => new PermissionDTO
            {
                Id = rp.Permission.Id,
                Function = rp.Permission.Function,
                PermissionGroupId = rp.Permission.PermissionGroupId,
                PermissionGroupName = rp.Permission.PermissionGroup.Name,
                Command = rp.Permission.Command,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description,
                IsAssigned = true
            }).ToList();
        }
    }
}
