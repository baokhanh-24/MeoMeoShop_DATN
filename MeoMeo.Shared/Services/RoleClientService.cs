using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services
{
    public class RoleClientService : IRoleClientService
    {
        private readonly IApiCaller _apiCaller;

        public RoleClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<RoleDTO>>("api/role");
                return response ?? new List<RoleDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting roles: {ex.Message}");
                return new List<RoleDTO>();
            }
        }

        public async Task<RoleDTO> GetRoleByIdAsync(Guid id)
        {
            try
            {
                var response = await _apiCaller.GetAsync<RoleDTO>($"api/role/{id}");
                return response ?? new RoleDTO();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting role: {ex.Message}");
                return new RoleDTO();
            }
        }

        public async Task<BaseResponse> CreateRoleAsync(CreateOrUpdateRoleDTO dto)
        {
            try
            {
                var response = await _apiCaller.PostAsync<CreateOrUpdateRoleDTO, BaseResponse>("api/role", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdateRoleAsync(CreateOrUpdateRoleDTO dto)
        {
            try
            {
                var response = await _apiCaller.PutAsync<CreateOrUpdateRoleDTO, BaseResponse>("api/role", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeleteRoleAsync(Guid id)
        {
            try
            {
                var response = await _apiCaller.DeleteAsync($"api/role/{id}");
                return new BaseResponse { ResponseStatus = response ? BaseStatus.Success : BaseStatus.Error, Message = response ? "" : "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> AssignPermissionsToRoleAsync(AssignPermissionsToRoleDTO dto)
        {
            try
            {
                var response = await _apiCaller.PostAsync<AssignPermissionsToRoleDTO, BaseResponse>("api/role/assign-permissions", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning permissions to role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<PermissionGroupDTO>> GetRolePermissionsTreeAsync(Guid roleId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<PermissionGroupDTO>>($"api/role/{roleId}/permissions-tree");
                return response ?? new List<PermissionGroupDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting role permissions tree: {ex.Message}");
                return new List<PermissionGroupDTO>();
            }
        }

        public async Task<List<UserRoleDTO>> GetRoleUsersAsync(Guid roleId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<UserRoleDTO>>($"api/role/{roleId}/users");
                return response ?? new List<UserRoleDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting role users: {ex.Message}");
                return new List<UserRoleDTO>();
            }
        }
    }
}
