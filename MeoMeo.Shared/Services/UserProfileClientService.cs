using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class UserProfileClientService : IUserProfileClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<UserProfileClientService> _logger;

        public UserProfileClientService(IApiCaller httpClient, ILogger<UserProfileClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDTO?> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync<UserDTO>("api/users/get-current-user");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy thông tin user hiện tại: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<BaseResponse> UpdateUserProfileAsync(CreateOrUpdateUserDTO user)
        {
            try
            {
                var url = $"api/users/update-user-async";
                var result = await _httpClient.PutAsync<CreateOrUpdateUserDTO, BaseResponse>(url, user);
                return result ?? new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật profile user: {Message}", ex.Message);
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật profile"
                };
            }
        }

        public async Task<BaseResponse> ChangePasswordAsync(ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var url = "api/users/change-password-async";
                var result = await _httpClient.PutAsync<ChangePasswordDTO, BaseResponse>(url, changePasswordDto);
                return result ?? new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi đổi mật khẩu: {Message}", ex.Message);
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi đổi mật khẩu"
                };
            }
        }
    }
}
