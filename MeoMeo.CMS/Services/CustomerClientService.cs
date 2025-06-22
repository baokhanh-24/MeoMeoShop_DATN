using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Utilities;

namespace MeoMeo.CMS.Services
{
    public class CustomerClientService : ICustomerClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<CustomerClientService> _logger;

        public CustomerClientService(IApiCaller httpClient, ILogger<CustomerClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(GetListCustomerRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Customer/get-all-customer-async?{queryString}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<CustomerDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<CustomerDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách Customer: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<CustomerDTO>();
            }
        }

        public async Task<CustomerDTO> GetCustomersByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/Customer/find-customer-by-id-async/{id}";
                var response = await _httpClient.GetAsync<CustomerDTO>(url);
                return response ?? new CustomerDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy Customer Id {Id}: {Message}", id, ex.Message);
                return new CustomerDTO();
            }
        }

        public async Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(CreateOrUpdateCustomerDTO createOrUpdateCustomerDto)
        {
            try
            {
                var url = "/api/Customer/create-customer-async";
                var result = await _httpClient.PostAsync<CreateOrUpdateCustomerDTO, CreateOrUpdateCustomerResponse>(url, createOrUpdateCustomerDto);
                return result ?? new CreateOrUpdateCustomerResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo Customer: {Message}", ex.Message);
                return new CreateOrUpdateCustomerResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(CreateOrUpdateCustomerDTO createOrUpdateCustomerDto)
        {
            try
            {
                var url = $"/api/Customer/update-customer-async/{createOrUpdateCustomerDto.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdateCustomerDTO, CreateOrUpdateCustomerResponse>(url, createOrUpdateCustomerDto);
                return result ?? new CreateOrUpdateCustomerResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật Customer {Id}: {Message}", createOrUpdateCustomerDto.Id, ex.Message);
                return new CreateOrUpdateCustomerResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            try
            {
                var url = $"/api/Customer/delete-customer-async/{id}";
                var success = await _httpClient.DeleteAsync(url);
                if (!success)
                {
                    _logger.LogWarning("Xoá Customer thất bại với Id {Id}", id);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xoá Customer {Id}: {Message}", id, ex.Message);
                return false;
            }
        }
    }
}
