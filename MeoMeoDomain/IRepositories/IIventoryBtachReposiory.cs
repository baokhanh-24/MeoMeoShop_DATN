using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IIventoryBtachReposiory : IBaseRepository<InventoryBatch>
    {
        Task<List<InventoryBatch>> GetAllBatchAsync();
        Task<InventoryBatch> GetBatchByIdAsync(Guid id);
        Task<InventoryBatch> CreateAsync(InventoryBatch inventoryBatch);
        Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatch inventoryBatch);
        Task<bool> DeleteInventoryBatchAsync(Guid id);
    }
}
