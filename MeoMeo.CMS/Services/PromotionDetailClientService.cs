using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;


namespace MeoMeo.CMS.Services
{
    public class PromotionDetailClientService : IPromotionDetailClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<PromotionDetailClientService> _logger;

        public PromotionDetailClientService(IApiCaller httpClient, ILogger<PromotionDetailClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>> GetAllPromotionDetailAsync(GetListPromotionDetailRequestDTO request)
        {
            var query = BuildQuery.ToQueryString(request);
            var url = $"/api/PromotionDetails/get-all-promotion-detail-async?{query}";
            var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>>(url);
            return response ?? new PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>();
        }

        public async Task<CreateOrUpdatePromotionDetailDTO> GetPromotionDetailByIdAsync(Guid id)
        {
            var url = $"/api/PromotionDetails/find-promotion-detail-by-id-async/{id}";
            var response = await _httpClient.GetAsync<CreateOrUpdatePromotionDetailDTO>(url);
            return response;
        }

        public async Task<CreateOrUpdatePromotionDetailResponseDTO> CreateAsync(CreateOrUpdatePromotionDetailDTO dto)
        {
            try
            {
                var url = "/api/PromotionDetails/create-promotion-detail-async";
                var result = await _httpClient.PostAsync<CreateOrUpdatePromotionDetailDTO, CreateOrUpdatePromotionDetailResponseDTO>(url, dto);
                return result ?? new CreateOrUpdatePromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo PromotionDetail: {Message}", ex.Message);
                return new CreateOrUpdatePromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<CreateOrUpdatePromotionDetailResponseDTO> UpdateAsync(CreateOrUpdatePromotionDetailDTO dto)
        {
            try
            {
                var url = $"/api/PromotionDetails/update-promotion-detail-async/{dto.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdatePromotionDetailDTO, CreateOrUpdatePromotionDetailResponseDTO>(url, dto);
                return result ?? new CreateOrUpdatePromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật PromotionDetail {Id}: {Message}", dto.Id, ex.Message);
                return new CreateOrUpdatePromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var url = $"/api/PromotionDetails/delete-promotion-detail-async/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Xoá PromotionDetail thất bại với Id {Id}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá PromotionDetail {Id}: {Message}", id, ex.Message);
                return false;
            }
        }
    }
}
