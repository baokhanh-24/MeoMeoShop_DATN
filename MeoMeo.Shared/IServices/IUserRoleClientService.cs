using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.IServices
{
    public interface IUserRoleClientService
    {
        Task<List<UserWithRolesDTO>> GetAllUsersWithRolesAsync();
        Task<UserWithRolesDTO> GetUserWithRolesAsync(Guid userId);
        Task<BaseResponse> AssignRoleToUserAsync(AssignRoleToUserDTO dto);
        Task<BaseResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
        Task<List<UserRoleDTO>> GetUserRolesAsync(Guid userId);
        Task<List<UserRoleDTO>> GetRoleUsersAsync(Guid roleId);
    }
}
