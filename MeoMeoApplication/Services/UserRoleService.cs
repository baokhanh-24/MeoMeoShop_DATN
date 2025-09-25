using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(
            IUserRoleRepository userRoleRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserWithRolesDTO>> GetAllUsersWithRolesAsync()
        {
            var users = await _userRepository.Query()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new UserWithRolesDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    IsLocked = u.IsLocked,
                    Status = u.Status,
                    Roles = u.UserRoles.Select(ur => new RoleDTO
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        Description = ur.Role.Description,
                        Status = ur.Role.Status
                    }).ToList()
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserWithRolesDTO> GetUserWithRolesAsync(Guid userId)
        {
            var user = await _userRepository.Query()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == userId)
                .Select(u => new UserWithRolesDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    IsLocked = u.IsLocked,
                    Status = u.Status,
                    Roles = u.UserRoles.Select(ur => new RoleDTO
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        Description = ur.Role.Description,
                        Status = ur.Role.Status
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return user ?? new UserWithRolesDTO();
        }

        public async Task<BaseResponse> AssignRoleToUserAsync(AssignRoleToUserDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Check if user exists
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy người dùng" };
                }

                // Check if role exists
                var role = await _roleRepository.GetByIdAsync(dto.RoleId);
                if (role == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy vai trò" };
                }

                // Get all existing roles for this user
                var existingRoles = await _userRoleRepository.Query()
                    .Where(ur => ur.UserId == dto.UserId)
                    .ToListAsync();

                // Check if user already has this role
                var hasSameRole = existingRoles.Any(ur => ur.RoleId == dto.RoleId);
                if (hasSameRole && existingRoles.Count == 1)
                {
                    await _unitOfWork.RollbackAsync();
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Người dùng đã có vai trò này" };
                }

                // Remove all existing roles for this user
                if (existingRoles.Any())
                {
                    // Use DeleteRangeAsync for better performance with composite keys
                    await _userRoleRepository.DeleteRangeAsync(existingRoles);
                }

                // Add new role
                var newUserRole = new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId
                };

                await _userRoleRepository.AddAsync(newUserRole);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Cập nhật vai trò cho người dùng thành công" };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var userRole = await _userRoleRepository.Query()
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy vai trò của người dùng" };
                }

                // Use RemoveUserRole for composite key entity
                _userRoleRepository.RemoveUserRole(userRole);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Xóa vai trò khỏi người dùng thành công" };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<UserRoleDTO>> GetUserRolesAsync(Guid userId)
        {
            var userRoles = await _userRoleRepository.Query()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
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
