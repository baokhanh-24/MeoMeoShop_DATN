using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
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
                var url = $"/api/Customers/get-all-customer-async?{queryString}";
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
                var url = $"/api/Customers/find-customer-by-id-async/{id}";
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
                var url = "/api/Customers/create-customer-async";
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
                var url = $"/api/Customers/update-customer-async/{createOrUpdateCustomerDto.Id}";
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
        public async Task<CreateOrUpdateCustomerResponse> UpdateProfileAsync(CreateOrUpdateCustomerDTO createOrUpdateCustomerDto)
        {
            try
            {
                var url = $"/api/Customers/update-profile";
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

        public async Task<BaseResponse> DeleteCustomersAsync(Guid id)
        {
            try
            {
                var url = $"/api/Customers/delete-customer-async/{id}";
                var success = await _httpClient.DeleteAsync(url);
                if (!success)
                {
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Xoá khách hàng thất bại" };
                }
                return new BaseResponse { ResponseStatus = BaseStatus.Success, Message = "Xóa khách hàng thành công" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xoá Customer {Id}: {Message}", id, ex.Message);
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }



        public async Task<BaseResponse> UploadAvatarAsync(IFormFile file)
        {
            try
            {
                var url = $"/api/Customers/upload-avatar-async";
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);
                var response = await _httpClient.PostFormAsync<BaseResponse>(url, content);
                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };

            }
        }

        public async Task<BaseResponse> ChangePasswordAsync(ChangePasswordDTO model)
        {
            try
            {
                var url = "/api/Customers/change-password-async";
                var response = await _httpClient.PutAsync<ChangePasswordDTO, BaseResponse>(url, model);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi đổi mật khẩu cho Customer {CustomerId}: {Message}", model.CustomerId, ex.Message);
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<QuickCustomerResponseDTO> CreateQuickCustomerAsync(CreateQuickCustomerDTO request)
        {
            try
            {
                var url = "/api/Customers/create-quick-customer-async";
                var response = await _httpClient.PostAsync<CreateQuickCustomerDTO, QuickCustomerResponseDTO>(url, request);
                return response ?? new QuickCustomerResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không thể tạo khách hàng"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo nhanh Customer: {Message}", ex.Message);
                return new QuickCustomerResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }
    }
}
