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
                var success = await _apiCaller.DeleteAsync($"api/role/{id}");
                return new BaseResponse
                {
                    ResponseStatus = success ? BaseStatus.Success : BaseStatus.Error,
                    Message = success ? "Xóa vai trò thành công" : "Có lỗi xảy ra"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> AssignUserToRoleAsync(AssignUserToRoleDTO dto)
        {
            try
            {
                var response = await _apiCaller.PostAsync<AssignUserToRoleDTO, BaseResponse>("api/role/assign-user", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning user to role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> RemoveUserFromRoleAsync(AssignUserToRoleDTO dto)
        {
            try
            {
                var success = await _apiCaller.DeleteAsync($"api/role/remove-user?userId={dto.UserId}&roleId={dto.RoleId}");
                return new BaseResponse
                {
                    ResponseStatus = success ? BaseStatus.Success : BaseStatus.Error,
                    Message = success ? "Xóa vai trò thành công" : "Có lỗi xảy ra"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing user from role: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }
    }
}
