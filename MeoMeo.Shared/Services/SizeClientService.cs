using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class SizeClientService : ISizeClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<SizeClientService> _logger;
        public SizeClientService(IApiCaller httpClient, ILogger<SizeClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<PagingExtensions.PagedResult<SizeDTO>> GetAllSizeAsync(GetListSizeRequestDTO request)
        {
            try
            {
                var querry = BuildQuery.ToQueryString(request);
                var url = $"api/Size?{querry}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<SizeDTO>>(url);
                _logger.LogInformation("Calling URL: {url}", url);
                return response ?? new PagingExtensions.PagedResult<SizeDTO>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách Size: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<SizeDTO>();
            }
        }
        public async Task<SizeDTO> GetSizeByIdAsync(Guid id)
        {
            try
            {
                var url = $"api/Size/{id}";
                var response = await _httpClient.GetAsync<SizeDTO>(url);
                return response ?? new SizeDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy Size Id {Id}: {Message}", id, ex.Message);
                return new SizeDTO();
            }
        }
        public async Task<SizeResponseDTO> CreateSizeAsync(SizeDTO size)
        {
            try
            {
                var url = "api/Size";
                var result = await _httpClient.PostAsync<SizeDTO, SizeResponseDTO>(url, size);
                return result ?? new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo Size: {Message}", ex.Message);
                return new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo Size"
                };
            }
        }
        public async Task<SizeResponseDTO> UpdateSizeAsync(SizeDTO size)
        {
            try
            {
                var url = $"api/Size/{size.Id}";
                var result = await _httpClient.PutAsync<SizeDTO, SizeResponseDTO>(url, size);
                _logger.LogInformation("Size ID: " + size.Id);
                _logger.LogInformation("Calling PUT URL: " + url);
                _logger.LogInformation("Size ID: " + size.Id);

                return result ?? new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật Size: {Message}", ex.Message);
                return new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật Size"
                };
            }
        }

        public async Task<bool> DeleteSizeAsync(Guid id)
        {
            try
            {
                var url = $"api/Size/{id}";
                var success = await _httpClient.DeleteAsync(url);
                if(!success)
                {
                    _logger.LogWarning("Không thể xóa kích thước với Id {Id}", id);
                }
                return success; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xoá Size Id {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<SizeResponseDTO> UpdateSizeStatusAsync(UpdateSizeStatusRequestDTO dto)
        {
            try
            {
                var url = "api/Size/update-size-status";
                var result = await _httpClient.PutAsync<UpdateSizeStatusRequestDTO, SizeResponseDTO>(url, dto);
                return result ?? new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật trạng thái Size: {Message}", ex.Message);
                return new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái Size"
                };
            }
        }
    }
}
