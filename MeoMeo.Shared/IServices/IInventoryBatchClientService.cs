using AntDesign;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.InventoryBatch;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Shared.IServices
{
    public interface IInventoryBatchClientService
    {
        Task<PagingExtensions.PagedResult<InventoryBatchDTO, GetListInventoryBatchResponseDTO>> GetAllInventoryBatchAsync(GetListInventoryBatchRequestDTO filter);
        Task<InventoryBatchDTO> GetInventoryBatchByIdAsync(Guid id);
        Task<InventoryBatchResponseDTO> CreateInventoryBatchAsync(List<InventoryBatchDTO> dto);
        Task<InventoryBatchResponseDTO> UpdateInventoryBatchAsync(InventoryBatchDTO dto);
        Task<bool> DeleteInventoryBatchAsync(Guid id);
    }
}
