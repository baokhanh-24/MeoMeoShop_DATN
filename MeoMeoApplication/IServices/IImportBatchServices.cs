using MeoMeo.Contract.DTOs.ImportBatch;
using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Application.IServices
{
    public interface IImportBatchServices
    {
        Task<ImportBatchResponseDTO> CreateAsync(ImportBatchDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>> GetAllAsync(GetListImportBatchRequestDTO request);
        Task<ImportBatchResponseDTO> GetByIdAsync(Guid id);
        Task<ImportBatchDetailDTO> GetDetailByIdAsync(Guid id);
        Task<ImportBatchResponseDTO> UpdateAsync(Guid id, ImportBatchDTO dto);
    }
}
