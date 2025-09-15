using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employee;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class EmployeesClientService : IEmployeesClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<EmployeesClientService> _logger;
        public EmployeesClientService(IApiCaller httpClient, ILogger<EmployeesClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> ChangePasswordAsync(ChangePasswordRequestDTO dto)
        {
            try
            {
                var url = $"api/Employees/change-password-async";
                var result = await _httpClient.PutAsync<ChangePasswordRequestDTO, CreateOrUpdateEmployeeResponseDTO>(url, dto);
                return result ?? new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi đổi mật khẩu: {Message}", ex.Message);
                return new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi đổi mật khẩu s"
                };
            }
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> CreateEmployeesAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                var url = $"api/Employees/create-employee-async";
                var result = await _httpClient.PostAsync<CreateOrUpdateEmployeeDTO, CreateOrUpdateEmployeeResponseDTO>(url, employee);
                return result ?? new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo nhân viên: {Message}", ex.Message);
                return new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo nhân viên"
                };
            }
        }

        public async Task<bool> DeleteEmployeesAsync(Guid id)
        {
            try
            {
                var url = $"api/Employees/delete-employee-async/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Không thể xóa nhân viên với id {id}: ", id);

                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa nhân viên với Id {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>> GetAllEmployeesAsync(GetlistEmployeesRequestDTO request)
        {
            try
            {
                var query = BuildQuery.ToQueryString(request);
                var url = $"api/Employees/get-all-employee-async?{query}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách nhân viên: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>();
            }
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeesByIdAsync(Guid id)
        {
            try
            {
                var url = $"api/Employees/find-employee-by-id-async/{id}";
                var result = await _httpClient.GetAsync<CreateOrUpdateEmployeeResponseDTO>(url);
                return result ?? new CreateOrUpdateEmployeeResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy nhân viên với Id {Id}: {Message}", id, ex.Message);
                return new CreateOrUpdateEmployeeResponseDTO();
            }
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeesAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                var url = $"api/Employees/update-employee-async/{employee.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdateEmployeeDTO, CreateOrUpdateEmployeeResponseDTO>(url, employee);
                return result ?? new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật nhân viên: {Message}", ex.Message);
                return new CreateOrUpdateEmployeeResponseDTO();
            }
        }

        public async Task<BaseResponse> UploadAvatarAsync(IFormFile file)
        {
            try
            {
                var url = "api/Employees/upload-avatar-async";
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "AvatarFile", file.FileName);
                var response = await _httpClient.PostFormAsync<BaseResponse>(url, content);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tải avatar: {Message}", ex.Message);
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> ChangePasswordAsync(ChangePasswordDTO model)
        {
            try
            {
                var url = "api/Employees/change-password-async";
                var response = await _httpClient.PutAsync<ChangePasswordDTO, BaseResponse>(url, model);
                return response ?? new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi đổi mật khẩu: {Message}", ex.Message);
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> UpdateProfileAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                var url = "api/Employees/update-profile";
                var result = await _httpClient.PutAsync<CreateOrUpdateEmployeeDTO, CreateOrUpdateEmployeeResponseDTO>(url, employee);
                return result ?? new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật profile: {Message}", ex.Message);
                return new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<string> GetAvatarUrlAsync()
        {
            try
            {
                var url = "api/Employees/get-avatar-url";
                var response = await _httpClient.GetAsync<AvatarUrlResponseDTO>(url);
                return response?.AvatarUrl ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy avatar URL: {Message}", ex.Message);
                return string.Empty;
            }
        }
    }
}
