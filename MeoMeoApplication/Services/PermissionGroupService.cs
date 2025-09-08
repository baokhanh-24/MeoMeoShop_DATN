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

        public async Task<BaseResponse> CreatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto)
        {
            try
            {
                var group = new PermissionGroup
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    ParentId = dto.ParentId,
                    Order = dto.Order
                };

                await _permissionGroupRepository.AddAsync(group);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Tạo nhóm quyền thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto)
        {
            try
            {
                var group = await _permissionGroupRepository.GetByIdAsync(dto.Id!.Value);
                if (group == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy nhóm quyền" };

                group.Name = dto.Name;
                group.Description = dto.Description;
                group.ParentId = dto.ParentId;
                group.Order = dto.Order;
                await _permissionGroupRepository.UpdateAsync(group);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Cập nhật nhóm quyền thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeletePermissionGroupAsync(Guid id)
        {
            try
            {
                var group = await _permissionGroupRepository.GetByIdAsync(id);
                if (group == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy nhóm quyền" };

                // Check if group has permissions
                var hasPermissions = await _permissionRepository.Query()
                    .AnyAsync(p => p.PermissionGroupId == id);

                if (hasPermissions)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không thể xóa nhóm quyền có chứa quyền" };

                // Check if group has children
                var hasChildren = await _permissionGroupRepository.Query()
                    .AnyAsync(pg => pg.ParentId == id);

                if (hasChildren)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không thể xóa nhóm quyền có nhóm con" };

                await _permissionGroupRepository.DeleteAsync(group.Id);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Xóa nhóm quyền thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }
    }
}
