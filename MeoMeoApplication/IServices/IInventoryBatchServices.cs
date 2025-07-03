using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IInventoryBatchServices
    {
        Task<PagingExtensions.PagedResult<InventoryBatchDTO>> GetAllAsync(GetListInventoryBatchRequestDTO request);
        Task<InventoryBatchResponseDTO> GetByIdAsync(Guid id);
        Task<InventoryBatchResponseDTO> CreateAsync(List<InventoryBatchDTO> dto);
        Task<InventoryBatchResponseDTO> UpdateAsync(Guid id, InventoryBatchDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
