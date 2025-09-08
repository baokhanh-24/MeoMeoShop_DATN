using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services
{
    public class UserRoleClientService : IUserRoleClientService
    {
        private readonly IApiCaller _apiCaller;

        public UserRoleClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<List<UserWithRolesDTO>> GetAllUsersWithRolesAsync()
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<UserWithRolesDTO>>("api/userrole/users");
                return response ?? new List<UserWithRolesDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users with roles: {ex.Message}");
                return new List<UserWithRolesDTO>();
            }
        }

        public async Task<UserWithRolesDTO> GetUserWithRolesAsync(Guid userId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<UserWithRolesDTO>($"api/userrole/users/{userId}");
                return response ?? new UserWithRolesDTO();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user with roles: {ex.Message}");
                return new UserWithRolesDTO();
            }
        }

        public async Task<BaseResponse> AssignRoleToUserAsync(AssignRoleToUserDTO dto)
        {
            try
            {
                var response = await _apiCaller.PostAsync<AssignRoleToUserDTO, BaseResponse>("api/userrole/assign", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning role to user: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
        {
            try
            {
                var response = await _apiCaller.DeleteAsync($"api/userrole/users/{userId}/roles/{roleId}");
                return new BaseResponse { ResponseStatus = response ? BaseStatus.Success : BaseStatus.Error, Message = response ? "Xóa thành công" : "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing role from user: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<UserRoleDTO>> GetUserRolesAsync(Guid userId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<UserRoleDTO>>($"api/userrole/users/{userId}/roles");
                return response ?? new List<UserRoleDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user roles: {ex.Message}");
                return new List<UserRoleDTO>();
            }
        }

        public async Task<List<UserRoleDTO>> GetRoleUsersAsync(Guid roleId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<UserRoleDTO>>($"api/userrole/roles/{roleId}/users");
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
