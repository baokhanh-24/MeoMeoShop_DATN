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
        Task<CreateOrUpdateInvetoryBatchResponse> GetByIdAsync(Guid id);
        Task<CreateOrUpdateInvetoryBatchResponse> CreateAsync(InventoryBatchDTO dto);
        Task<CreateOrUpdateInvetoryBatchResponse> UpdateAsync(Guid id, InventoryBatchDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
