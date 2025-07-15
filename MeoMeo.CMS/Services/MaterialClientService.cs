using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.CMS.Services
{
    public class MaterialClientService : IMaterialClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<MaterialClientService> _logger;
        public MaterialClientService(IApiCaller httpClient, ILogger<MaterialClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<CreateOrUpdateMaterialResponse> CreateMaterialsAsync(CreateOrUpdateMaterialDTO material)
        {
            try
            {
                var url = $"api/Materials";
                var result = await _httpClient.PostAsync<CreateOrUpdateMaterialDTO, CreateOrUpdateMaterialResponse>(url, material);
                return result ?? new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo chất liệu: {Message}", ex.Message);
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo chất liệu"
                };
            }
        }

        public async Task<bool> DeleteMaterialsAsync(Guid id)
        {
            try
            {
                var url = $"api/Materials/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Không thể xóa chất liệu với Id {Id}: ", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa chất liệu với Id {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>> GetAllMaterialsAsync(GetListMaterialRequest request)
        {
            try
            {
                var query = BuildQuery.ToQueryString(request);
                Console.WriteLine("Query generated: " + query);
                var url = $"api/Materials?{query}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách chất liệu: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>();
            }
        }

        public async Task<CreateOrUpdateMaterialDTO> GetMaterialsByIdAsync(Guid id)
        {
            try
            {
                var url = $"api/Materials/{id}";
                var response = await _httpClient.GetAsync<CreateOrUpdateMaterialDTO>(url);
                return response ?? new CreateOrUpdateMaterialDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy chất liệu Id {Id}: {Message}", id, ex.Message);
                return new CreateOrUpdateMaterialDTO();
            }
        }

        public async Task<CreateOrUpdateMaterialResponse> UpdateMaterialsAsync(CreateOrUpdateMaterialDTO material)
        {
            try
            {
                var url = $"api/Materials/{material.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdateMaterialDTO, CreateOrUpdateMaterialResponse>(url, material);
                _logger.LogInformation("Material ID: " + material.Id);
                _logger.LogInformation("Calling PUT URL: " + url);
                _logger.LogInformation("Material ID: " + material.Id);
                return result ?? new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật chất liệu: {Message}", ex.Message);
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật chất liệu"
                };
            }
        }

        public async Task<CreateOrUpdateMaterialResponse> UpdateMaterialStatusAsync(UpdateStatusRequestDTO dto)
        {
            try
            {
                var url = "api/Materials/update-status";
                var result = await _httpClient.PutAsync<UpdateStatusRequestDTO, CreateOrUpdateMaterialResponse>(url, dto);
                return result ?? new CreateOrUpdateMaterialResponse { ResponseStatus = BaseStatus.Error, Message = "Không có phản hồi từ server." };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi gọi API: {ex.Message}"
                };
            }
        }
    }
}
