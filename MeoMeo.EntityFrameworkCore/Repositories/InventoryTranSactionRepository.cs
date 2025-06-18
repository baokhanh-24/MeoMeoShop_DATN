using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class InventoryTranSactionRepository : BaseRepository<InventoryTransaction>, IInventoryTranSactionRepository
    {
        public InventoryTranSactionRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<InventoryTransaction> CreateAsync(InventoryTransaction inventoryTransaction)
        {
            try
            {
                await AddAsync(inventoryTransaction);
                return inventoryTransaction;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the inventory transaction.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var phat = await GetByIdAsync(id);
            if (phat == null)
            {
                return false;
            }
            await base.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetAllTransactionAsync()
        {
            return await GetAllAsync();
        }

        public async Task<InventoryTransaction> GetTransactionByIdAsync(Guid id)
        {
            var phat = await GetByIdAsync(id);
            return phat;
        }

        public async Task<InventoryTransaction> UpdateAsync(Guid id, InventoryTransaction inventoryTransaction)
        {
            var phat = await GetByIdAsync(id);
            if (phat == null)
            {
                throw new Exception("Inventory transaction not found.");
            }
            phat.InventoryBatchId = inventoryTransaction.InventoryBatchId;
            phat.Quantity = inventoryTransaction.Quantity;
            phat.CreationTime = inventoryTransaction.CreationTime;
            phat.CreateBy = inventoryTransaction.CreateBy;
            phat.Type = inventoryTransaction.Type;
            phat.Note = inventoryTransaction.Note;
            return await UpdateAsync(phat);
        }
    }
}
