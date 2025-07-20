using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.InventoryBatch;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class PromotionClientService : IPromotionClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<PromotionClientService> _logger;
        public PromotionClientService(IApiCaller httpClient, ILogger<PromotionClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        //
        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>> GetAllPromotionAsync(GetListPromotionRequestDTO request)
        {
            var query = BuildQuery.ToQueryString(request);
            var url = $"/api/Promotions/get-all-promotion-async?{query}";
            var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>>(url);
            return response ?? new PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>();
        }
        public async Task<CreateOrUpdatePromotionDTO> GetPromotionByIdAsync(Guid id)
        {
            var url = $"/api/Promotions/find-promotion-by-id-async/{id}";
            var response = await _httpClient.GetAsync<CreateOrUpdatePromotionDTO>(url);
            return response;
        }
        public async Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionAsync(CreateOrUpdatePromotionDTO dto)
        {
            try
            {
                var url = "/api/Promotions/create-promotion-async";
                var result = await _httpClient.PostAsync<CreateOrUpdatePromotionDTO, CreateOrUpdatePromotionResponseDTO>(url, dto);
                return result ?? new CreateOrUpdatePromotionResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo Promotion: {Message}", ex.Message);
                return new CreateOrUpdatePromotionResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionAsync(CreateOrUpdatePromotionDTO dto)
        {
            try
            {
                var url = $"/api/Promotions/update-promotion-async/{dto.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdatePromotionDTO, CreateOrUpdatePromotionResponseDTO>(url, dto);
                return result ?? new CreateOrUpdatePromotionResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật InventoryBatch {Id} : {Message}", dto.Id, ex.Message);
                return new CreateOrUpdatePromotionResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }
        public async Task<bool> DeletePromotionAsync(Guid id)
        {
            try
            {
                var url = $"/api/Promotions/delete-promotion-async/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Xóa InventoryBatch thất bại với Id {Id}", id);
                }
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa InventoryBatch {Id} : {Message}", id, ex.Message);
                return false;
            }
        }
    }
    
}
