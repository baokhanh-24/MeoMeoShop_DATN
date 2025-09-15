using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ImportBatch;
using MeoMeo.Shared.Utilities;
using MeoMeo.Domain.Commons;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class ImportBatchClientService : IImportBatchClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<ImportBatchClientService> _logger;

        public ImportBatchClientService(IApiCaller httpClient, ILogger<ImportBatchClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>> GetAllImportBatchAsync(GetListImportBatchRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/ImportBatch/get-all-import-batch-async?{queryString}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách ImportBatch: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>();
            }
        }

        public async Task<ImportBatchDTO> GetImportBatchByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/ImportBatch/get-import-batch-by-id-async/{id}";
                var response = await _httpClient.GetAsync<ImportBatchDTO>(url);
                return response ?? new ImportBatchDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy ImportBatch Id {Id}: {Message}", id, ex.Message);
                return new ImportBatchDTO();
            }
        }

        public async Task<ImportBatchDetailDTO> GetImportBatchDetailAsync(Guid id)
        {
            try
            {
                var url = $"/api/ImportBatch/get-import-batch-detail-async/{id}";
                var response = await _httpClient.GetAsync<ImportBatchDetailDTO>(url);
                return response ?? new ImportBatchDetailDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy ImportBatch Detail Id {Id}: {Message}", id, ex.Message);
                return new ImportBatchDetailDTO();
            }
        }

        public async Task<ImportBatchResponseDTO> CreateImportBatchAsync(ImportBatchDTO dto)
        {
            try
            {
                var url = "/api/ImportBatch/create-import-batch-async";
                var response = await _httpClient.PostAsync<ImportBatchDTO, ImportBatchResponseDTO>(url, dto);
                return response ?? new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không thể tạo ImportBatch" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo ImportBatch: {Message}", ex.Message);
                return new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<ImportBatchResponseDTO> UpdateImportBatchAsync(Guid id, ImportBatchDTO dto)
        {
            try
            {
                var url = $"/api/ImportBatch/update-import-batch-async/{id}";
                var response = await _httpClient.PutAsync<ImportBatchDTO, ImportBatchResponseDTO>(url, dto);
                return response ?? new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không thể cập nhật ImportBatch" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật ImportBatch Id {Id}: {Message}", id, ex.Message);
                return new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<bool> DeleteImportBatchAsync(Guid id)
        {
            try
            {
                var url = $"/api/ImportBatch/delete-import-batch-async/{id}";
                var response = await _httpClient.DeleteAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa ImportBatch Id {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<ImportBatchResponseDTO> ApproveImportBatchAsync(Guid id)
        {
            try
            {
                var url = $"/api/ImportBatch/approve-import-batch-async/{id}";
                var response = await _httpClient.PostAsync<Guid, ImportBatchResponseDTO>(url, id);
                return response ?? new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không thể duyệt ImportBatch" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi duyệt ImportBatch Id {Id}: {Message}", id, ex.Message);
                return new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<ImportBatchResponseDTO> RejectImportBatchAsync(Guid id)
        {
            try
            {
                var url = $"/api/ImportBatch/reject-import-batch-async/{id}";
                var response = await _httpClient.PostAsync<Guid, ImportBatchResponseDTO>(url, id);
                return response ?? new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không thể từ chối ImportBatch" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi từ chối ImportBatch Id {Id}: {Message}", id, ex.Message);
                return new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }
    }
}
