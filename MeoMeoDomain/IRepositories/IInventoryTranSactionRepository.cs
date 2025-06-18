using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IInventoryTranSactionRepository : IBaseRepository<InventoryTransaction>    
    {
        Task<IEnumerable<InventoryTransaction>> GetAllTransactionAsync();
        Task<InventoryTransaction> GetTransactionByIdAsync(Guid id);
        Task<InventoryTransaction> CreateAsync(InventoryTransaction inventoryTransaction);
        Task<InventoryTransaction> UpdateAsync(Guid id, InventoryTransaction inventoryTransaction);
        Task<bool> DeleteAsync(Guid id);
    }
}
