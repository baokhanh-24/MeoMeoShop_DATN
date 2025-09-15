using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ImportBatch;
using MeoMeo.Shared.Utilities;
using MeoMeo.Domain.Commons;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.IServices
{
    public interface IImportBatchClientService
    {
        Task<PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>> GetAllImportBatchAsync(GetListImportBatchRequestDTO request);
        Task<ImportBatchDTO> GetImportBatchByIdAsync(Guid id);
        Task<ImportBatchDetailDTO> GetImportBatchDetailAsync(Guid id);
        Task<ImportBatchResponseDTO> CreateImportBatchAsync(ImportBatchDTO dto);
        Task<ImportBatchResponseDTO> UpdateImportBatchAsync(Guid id, ImportBatchDTO dto);
        Task<bool> DeleteImportBatchAsync(Guid id);
        Task<ImportBatchResponseDTO> ApproveImportBatchAsync(Guid id);
        Task<ImportBatchResponseDTO> RejectImportBatchAsync(Guid id);
    }
}
