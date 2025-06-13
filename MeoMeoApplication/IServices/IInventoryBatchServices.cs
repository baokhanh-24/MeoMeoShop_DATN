using MeoMeo.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IInventoryBatchServices
    {
        Task<List<InventoryBatchDTO>> GetAllAsync();
        Task<InventoryBatchDTO> GetByIdAsync(Guid id);
        Task<InventoryBatchDTO> CreateAsync(InventoryBatchDTO dto);
        Task<InventoryBatchDTO> UpdateAsync(Guid id, InventoryBatchDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
