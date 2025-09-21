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
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.Query()
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status,
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
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
                .Where(r => r.Id == id)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status,
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

        public async Task<BaseResponse> AssignUserToRoleAsync(Guid userId, Guid roleId)
        {
            try
            {
                // Check if user already has this role
                var existingUserRole = await _userRoleRepository.Query()
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole != null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Người dùng đã có vai trò này" };

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                };

                await _userRoleRepository.AddAsync(userRole);

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Gán vai trò cho người dùng thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> RemoveUserFromRoleAsync(Guid userId, Guid roleId)
        {
            try
            {
                var userRole = await _userRoleRepository.Query()
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole == null)
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy vai trò của người dùng" };

                await _userRoleRepository.DeleteAsync(userRole);

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Xóa vai trò của người dùng thành công" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }
    }
}
