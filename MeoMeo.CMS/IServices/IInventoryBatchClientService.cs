using AntDesign;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.CMS.IServices
{
    public interface IInventoryBatchClientService
    {
        Task<PagingExtensions.PagedResult<InventoryBatchDTO>> GetAllInventoryBatchAsync(GetListInventoryBatchRequestDTO filter);
        Task<InventoryBatchDTO> GetInventoryBatchByIdAsync(Guid id);
        Task<InventoryBatchResponseDTO> CreateInventoryBatchAsync(List<InventoryBatchDTO> dto);
        Task<InventoryBatchResponseDTO> UpdateInventoryBatchAsync(InventoryBatchDTO dto);
        Task<bool> DeleteInventoryBatchAsync(Guid id);
    }
}
