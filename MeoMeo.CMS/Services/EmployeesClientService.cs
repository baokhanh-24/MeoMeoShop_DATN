using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.CMS.Services
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
                if(!result)
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
    }
}
