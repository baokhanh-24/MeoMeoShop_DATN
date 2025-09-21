using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.IServices
{
    public interface IRoleClientService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(Guid id);
        Task<BaseResponse> CreateRoleAsync(CreateOrUpdateRoleDTO dto);
        Task<BaseResponse> UpdateRoleAsync(CreateOrUpdateRoleDTO dto);
        Task<BaseResponse> DeleteRoleAsync(Guid id);
        Task<BaseResponse> AssignUserToRoleAsync(AssignUserToRoleDTO dto);
        Task<BaseResponse> RemoveUserFromRoleAsync(AssignUserToRoleDTO dto);
    }
}
