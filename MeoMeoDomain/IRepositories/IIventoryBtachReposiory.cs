using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IIventoryBtachReposiory
    {
        Task<List<InventoryBatch>> GetAllAsync();
        Task<InventoryBatch> GetByIdAsync(Guid id);
        Task<InventoryBatch> CreateAsync(InventoryBatch inventoryBatch);
        Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatch inventoryBatch);
        Task<bool> DeleteAsync(Guid id);
    }
}
