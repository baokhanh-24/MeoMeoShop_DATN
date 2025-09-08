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

        public async Task<BaseResponse> CreatePermissionAsync(CreateOrUpdatePermissionDTO dto)
        {
            try
            {
                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Function = dto.Function,
                    PermissionGroupId = dto.PermissionGroupId,
                    Command = dto.Command,
                    Name = dto.Name,
                    Description = dto.Description,
                };

                await _permissionRepository.AddAsync(permission);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Tạo quyền thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdatePermissionAsync(CreateOrUpdatePermissionDTO dto)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(dto.Id!.Value);
                if (permission == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy quyền" };

                permission.Function = dto.Function;
                permission.PermissionGroupId = dto.PermissionGroupId;
                permission.Command = dto.Command;
                permission.Name = dto.Name;
                permission.Description = dto.Description;

                await _permissionRepository.UpdateAsync(permission);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Cập nhật quyền thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeletePermissionAsync(Guid id)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(id);
                if (permission == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy quyền" };

                await _permissionRepository.DeleteAsync(permission.Id);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Xóa quyền thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

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
