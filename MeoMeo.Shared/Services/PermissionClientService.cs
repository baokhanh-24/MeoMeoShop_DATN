using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services
{
    public class PermissionClientService : IPermissionClientService
    {
        private readonly IApiCaller _apiCaller;

        public PermissionClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<List<PermissionDTO>> GetAllPermissionsAsync()
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<PermissionDTO>>("api/cms/permission");
                return response ?? new List<PermissionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permissions: {ex.Message}");
                return new List<PermissionDTO>();
            }
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                var response = await _apiCaller.GetAsync<PermissionDTO>($"api/cms/permission/{id}");
                return response ?? new PermissionDTO();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permission: {ex.Message}");
                return new PermissionDTO();
            }
        }

        public async Task<BaseResponse> CreatePermissionAsync(CreateOrUpdatePermissionDTO dto)
        {
            try
            {
                var response = await _apiCaller.PostAsync<CreateOrUpdatePermissionDTO, BaseResponse>("api/cms/permission", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating permission: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdatePermissionAsync(CreateOrUpdatePermissionDTO dto)
        {
            try
            {
                var response = await _apiCaller.PutAsync<CreateOrUpdatePermissionDTO, BaseResponse>("api/cms/permission", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating permission: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeletePermissionAsync(Guid id)
        {
            try
            {
                var response = await _apiCaller.DeleteAsync($"api/cms/permission/{id}");
                return  new BaseResponse { ResponseStatus = response ? BaseStatus.Success: BaseStatus.Error, Message =response ?  "":"Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting permission: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<PermissionDTO>> GetPermissionsByGroupIdAsync(Guid groupId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<PermissionDTO>>($"api/cms/permission/group/{groupId}");
                return response ?? new List<PermissionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permissions by group: {ex.Message}");
                return new List<PermissionDTO>();
            }
        }

        public async Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<PermissionDTO>>($"api/cms/permission/role/{roleId}");
                return response ?? new List<PermissionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permissions by role: {ex.Message}");
                return new List<PermissionDTO>();
            }
        }
    }
}
