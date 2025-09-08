using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services
{
    public class PermissionGroupClientService : IPermissionGroupClientService
    {
        private readonly IApiCaller _apiCaller;

        public PermissionGroupClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<List<PermissionGroupDTO>> GetAllPermissionGroupsAsync()
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<PermissionGroupDTO>>("api/permissiongroup");
                return response ?? new List<PermissionGroupDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permission groups: {ex.Message}");
                return new List<PermissionGroupDTO>();
            }
        }

        public async Task<List<PermissionGroupDTO>> GetPermissionGroupsTreeAsync()
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<PermissionGroupDTO>>("api/permissiongroup/tree");
                return response ?? new List<PermissionGroupDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permission groups tree: {ex.Message}");
                return new List<PermissionGroupDTO>();
            }
        }

        public async Task<PermissionGroupDTO> GetPermissionGroupByIdAsync(Guid id)
        {
            try
            {
                var response = await _apiCaller.GetAsync<PermissionGroupDTO>($"api/permissiongroup/{id}");
                return response ?? new PermissionGroupDTO();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permission group: {ex.Message}");
                return new PermissionGroupDTO();
            }
        }

        public async Task<BaseResponse> CreatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto)
        {
            try
            {
                var response = await _apiCaller.PostAsync<CreateOrUpdatePermissionGroupDTO, BaseResponse>("api/permissiongroup", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating permission group: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto)
        {
            try
            {
                var response = await _apiCaller.PutAsync<CreateOrUpdatePermissionGroupDTO, BaseResponse>("api/permissiongroup", dto);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating permission group: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeletePermissionGroupAsync(Guid id)
        {
            try
            {
                var response = await _apiCaller.DeleteAsync($"api/permissiongroup/{id}");
                return new BaseResponse { ResponseStatus = response ? BaseStatus.Success : BaseStatus.Error, Message = response ? "Xóa thành công" : "Có lỗi xảy ra" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting permission group: {ex.Message}");
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }
    }
}
