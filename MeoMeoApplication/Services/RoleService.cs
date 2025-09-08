using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IRolePermissionRepository rolePermissionRepository,
            IPermissionRepository permissionRepository,
            IUserRoleRepository userRoleRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.Query()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ThenInclude(p => p.PermissionGroup)
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDTO
                    {
                        Id = rp.Permission.Id,
                        Function = rp.Permission.Function,
                        PermissionGroupId = rp.Permission.PermissionGroupId,
                        PermissionGroupName = rp.Permission.PermissionGroup.Name,
                        Command = rp.Permission.Command,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description,
                        IsAssigned = true
                    }).ToList(),
                    UserRoles = r.UserRoles.Select(ur => new UserRoleDTO
                    {
                        UserId = ur.UserId,
                        UserName = ur.User.UserName,
                        Email = ur.User.Email,
                        Avatar = ur.User.Avatar,
                        RoleId = ur.RoleId,
                        RoleName = r.Name,
                        RoleDescription = r.Description
                    }).ToList()
                })
                .ToListAsync();

            return roles;
        }

        public async Task<RoleDTO> GetRoleByIdAsync(Guid id)
        {
            var role = await _roleRepository.Query()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ThenInclude(p => p.PermissionGroup)
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
                .Where(r => r.Id == id)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDTO
                    {
                        Id = rp.Permission.Id,
                        Function = rp.Permission.Function,
                        PermissionGroupId = rp.Permission.PermissionGroupId,
                        PermissionGroupName = rp.Permission.PermissionGroup.Name,
                        Command = rp.Permission.Command,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description,
                        IsAssigned = true
                    }).ToList(),
                    UserRoles = r.UserRoles.Select(ur => new UserRoleDTO
                    {
                        UserId = ur.UserId,
                        UserName = ur.User.UserName,
                        Email = ur.User.Email,
                        Avatar = ur.User.Avatar,
                        RoleId = ur.RoleId,
                        RoleName = r.Name,
                        RoleDescription = r.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return role ?? new RoleDTO();
        }

        public async Task<BaseResponse> CreateRoleAsync(CreateOrUpdateRoleDTO dto)
        {
            try
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    Status = dto.Status
                };

                await _roleRepository.AddAsync(role);

                // Assign permissions if provided
                if (dto.PermissionIds.Any())
                {
                    await AssignPermissionsToRoleAsync(new AssignPermissionsToRoleDTO
                    {
                        RoleId = role.Id,
                        PermissionIds = dto.PermissionIds
                    });
                }

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Tạo vai trò thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdateRoleAsync(CreateOrUpdateRoleDTO dto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(dto.Id!.Value);
                if (role == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy vai trò" };

                role.Name = dto.Name;
                role.Description = dto.Description;
                role.Status = dto.Status;

                await _roleRepository.UpdateAsync(role);

                // Update permissions if provided
                if (dto.PermissionIds.Any())
                {
                    await AssignPermissionsToRoleAsync(new AssignPermissionsToRoleDTO
                    {
                        RoleId = role.Id,
                        PermissionIds = dto.PermissionIds
                    });
                }

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Cập nhật vai trò thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeleteRoleAsync(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy vai trò" };

                // Check if role has users
                var hasUsers = await _userRoleRepository.Query()
                    .AnyAsync(ur => ur.RoleId == id);

                if (hasUsers)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không thể xóa vai trò đang được sử dụng" };

                await _roleRepository.DeleteAsync(role.Id);
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Xóa vai trò thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> AssignPermissionsToRoleAsync(AssignPermissionsToRoleDTO dto)
        {
            try
            {
                // Remove existing permissions
                await _rolePermissionRepository.DeleteByRoleIdAsync(dto.RoleId);

                // Add new permissions
                foreach (var permissionId in dto.PermissionIds)
                {
                    var rolePermission = new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = dto.RoleId,
                        PermissionId = permissionId
                    };

                    await _rolePermissionRepository.AddAsync(rolePermission);
                }

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Gán quyền cho vai trò thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<PermissionDTO>> GetRolePermissionsAsync(Guid roleId)
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

        public async Task<List<UserRoleDTO>> GetRoleUsersAsync(Guid roleId)
        {
            var userRoles = await _userRoleRepository.Query()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => new UserRoleDTO
                {
                    UserId = ur.UserId,
                    UserName = ur.User.UserName,
                    Email = ur.User.Email,
                    Avatar = ur.User.Avatar,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    RoleDescription = ur.Role.Description
                })
                .ToListAsync();

            return userRoles;
        }
    }
}
