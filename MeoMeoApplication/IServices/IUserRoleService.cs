using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.IServices
{
    public interface IUserRoleService
    {
        Task<List<UserWithRolesDTO>> GetAllUsersWithRolesAsync();
        Task<UserWithRolesDTO> GetUserWithRolesAsync(Guid userId);
        Task<BaseResponse> AssignRoleToUserAsync(AssignRoleToUserDTO dto);
        Task<BaseResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
        Task<List<UserRoleDTO>> GetUserRolesAsync(Guid userId);
        Task<List<UserRoleDTO>> GetRoleUsersAsync(Guid roleId);
    }
}
