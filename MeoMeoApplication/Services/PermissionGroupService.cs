using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class PermissionGroupService : IPermissionGroupService
    {
        private readonly IPermissionGroupRepository _permissionGroupRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionGroupService(
            IPermissionGroupRepository permissionGroupRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _permissionGroupRepository = permissionGroupRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<List<PermissionGroupDTO>> GetAllPermissionGroupsAsync()
        {
            var groups = await _permissionGroupRepository.Query()
                .Include(pg => pg.Parent)
                .Include(pg => pg.Permissions)
                .Select(pg => new PermissionGroupDTO
                {
                    Id = pg.Id,
                    Name = pg.Name,
                    Description = pg.Description,
                    ParentId = pg.ParentId,
                    ParentName = pg.Parent != null ? pg.Parent.Name : null,
                    Order = Convert.ToInt32(pg.Order),
                    Permissions = pg.Permissions.Select(p => new PermissionDTO
                    {
                        Id = p.Id,
                        Function = p.Function,
                        PermissionGroupId = p.PermissionGroupId,
                        Command = p.Command,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                })
                .ToListAsync();

            return groups;
        }

        public async Task<List<PermissionGroupDTO>> GetPermissionGroupsTreeAsync()
        {
            var allGroups = await GetAllPermissionGroupsAsync();
            var rootGroups = allGroups.Where(g => g.ParentId == null).OrderBy(g => g.Order).ToList();

            foreach (var rootGroup in rootGroups)
            {
                BuildTree(rootGroup, allGroups);
            }

            return rootGroups;
        }

        private void BuildTree(PermissionGroupDTO parent, List<PermissionGroupDTO> allGroups)
        {
            parent.Children = allGroups
                .Where(g => g.ParentId == parent.Id)
                .OrderBy(g => g.Order)
                .ToList();

            foreach (var child in parent.Children)
            {
                BuildTree(child, allGroups);
            }
        }

        public async Task<PermissionGroupDTO> GetPermissionGroupByIdAsync(Guid id)
        {
            var group = await _permissionGroupRepository.Query()
                .Include(pg => pg.Parent)
                .Include(pg => pg.Permissions)
                .Where(pg => pg.Id == id)
                .Select(pg => new PermissionGroupDTO
                {
                    Id = pg.Id,
                    Name = pg.Name,
                    Description = pg.Description,
                    ParentId = pg.ParentId,
                    ParentName = pg.Parent != null ? pg.Parent.Name : null,
                    Order = Convert.ToInt32(pg.Order),
                    Permissions = pg.Permissions.Select(p => new PermissionDTO
                    {
                        Id = p.Id,
                        Function = p.Function,
                        PermissionGroupId = p.PermissionGroupId,
                        Command = p.Command,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return group ?? new PermissionGroupDTO();
        }

        // CRUD operations removed - PermissionGroups are fixed in database
        // Only GET operations are allowed
    }
}
