using MeoMeo.Contract.DTOs;
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
        Task<IEnumerable<InventoryBatch>> GetAllAsync();
        Task<InventoryBatch> GetByIdAsync(Guid id);
        Task<InventoryBatch> CreateAsync(InventoryBatchDTO dto);
        Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatchDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
