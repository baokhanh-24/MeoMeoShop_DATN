using AntDesign;
using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MeoMeo.Shared.Utilities;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.CMS.Services
{
    public class InventoryBatchClientService : IInventoryBatchClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<InventoryBatchClientService> _logger;

        public InventoryBatchClientService(IApiCaller httpClient, ILogger<InventoryBatchClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        ///api/InventoryBatches
        public async Task<PagingExtensions.PagedResult<InventoryBatchDTO>> GetAllInventoryBatchAsync(GetListInventoryBatchRequestDTO filter)
        {
            var query = BuildQuery.ToQueryString(filter);
            var url = $"/api/InventoryBatches/get-all-inventoryBatch-async?{query}";
            var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<InventoryBatchDTO>>(url);
            return response ?? new PagingExtensions.PagedResult<InventoryBatchDTO>();
        }
        public async Task<InventoryBatchDTO> GetInventoryBatchByIdAsync(Guid id)
        {
            var url = $"/api/InventoryBatches/find-inventoryBatch-by-id-async/{id}";
            var response = await _httpClient.GetAsync<InventoryBatchDTO>(url);
            return response;
        }

        public async Task<InventoryBatchResponseDTO> CreateInventoryBatchAsync(List<InventoryBatchDTO> dto)
        {
            try
            {
                var url = "/api/InventoryBatches/create-inventoryBatch-async";
                var result = await _httpClient.PostAsync<List<InventoryBatchDTO>, InventoryBatchResponseDTO>(url, dto);
                return result ?? new InventoryBatchResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo InventotyBatch : {Message}", ex.Message);
                return new InventoryBatchResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<InventoryBatchResponseDTO> UpdateInventoryBatchAsync(InventoryBatchDTO dto)
        {
            try
            {
                var url = $"/api/InventoryBatches/update-inventoryBatch-async/{dto.Id}";
                var result = await _httpClient.PutAsync<InventoryBatchDTO, InventoryBatchResponseDTO>(url, dto);
                return result ?? new InventoryBatchResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật InventoryBatch {Id} : {Message}", dto.Id, ex.Message);
                return new InventoryBatchResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> DeleteInventoryBatchAsync(Guid id)
        {
            try
            {
                var url = $"/api/InventoryBatches/delete-inventoryBatch-async/{id}";
                var resutl = await _httpClient.DeleteAsync(url);
                if (!resutl)
                {
                    _logger.LogWarning("Xóa InventoryBatch thất bại với Id {Id}", id);
                }
                return resutl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa InventoryBatch {Id} : {Message}",id, ex.Message);
                return false;
            }
        }
    }
}
