
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.SystemConfig;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class SystenConfigClientService : ISystemConfigClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<SystenConfigClientService> _logger;
        public SystenConfigClientService(IApiCaller httpClient, ILogger<SystenConfigClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<CreateOrUpdateSystemConfigResponseDTO> CreateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig)
        {
            try
            {
                var url = $"api/SystemConfigs";
                var result = await _httpClient.PostAsync<CreateOrUpdateSystemConfigDTO, CreateOrUpdateSystemConfigResponseDTO>(url, systemConfig);
                return result ?? new CreateOrUpdateSystemConfigResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo cấu hình hệ thống: {Message}", ex.Message);
                return new CreateOrUpdateSystemConfigResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo cấu hình hệ thống"
                };
            }
        }

        public async Task<bool> DeleteSystemConfigAsync(Guid id)
        {
            try
            {
                var url = $"api/SystemConfigs{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Không thể xóa cấu hình hệ thống với Id {Id}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo cấu hình hệ thống: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdateSystemConfigDTO>> GetAllSystemConfigsAsync(GetListSystemConfigRequestDTO request)
        {
            try
            {
                var query = BuildQuery.ToQueryString(request);
                var url = $"api/SystemConfigs?{query}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<CreateOrUpdateSystemConfigDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<CreateOrUpdateSystemConfigDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách cấu hình hệ thống: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<CreateOrUpdateSystemConfigDTO>();
            }
        }

        public async Task<CreateOrUpdateSystemConfigDTO> GetSystemConfigByIdAsync(Guid id)
        {
            try
            {
                var url = $"api/SystemConfigs/{id}";
                var response = await _httpClient.GetAsync<CreateOrUpdateSystemConfigDTO>(url);
                return response ?? new CreateOrUpdateSystemConfigDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy cấu hình hệ thống Id {Id}: {Message}", id, ex.Message);
                return new CreateOrUpdateSystemConfigDTO();
            }
        }

        public async Task<CreateOrUpdateSystemConfigResponseDTO> UpdateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig)
        {
            try
            {
                var url = $"api/SystemConfigs/{systemConfig.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdateSystemConfigDTO, CreateOrUpdateSystemConfigResponseDTO>(url, systemConfig);
                return result ?? new CreateOrUpdateSystemConfigResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật cấu hình hệ thống: {Message}", ex.Message);
                return new CreateOrUpdateSystemConfigResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật cấu hình hệ thống"
                };
            }
        }

    }
}
