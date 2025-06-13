using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class InventoryTranSactionRepository : IInventoryTranSactionRepository
    {
        private readonly MeoMeoDbContext _context;
        public InventoryTranSactionRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task<InventoryTransaction> CreateAsync(InventoryTransaction inventoryTransaction)
        {
            try
            {
                _context.inventoryTransactions.Add(inventoryTransaction);
                await _context.SaveChangesAsync();
                return inventoryTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the inventory transaction.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var batch = await _context.inventoryTransactions.FindAsync(id);
                if (batch != null)
                {
                    _context.inventoryTransactions.Remove(batch);
                    await _context.SaveChangesAsync();

                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the inventory transaction.", ex);
            }
        }

        public async Task<List<InventoryTransaction>> GetAllAsync()
        {
            return await _context.inventoryTransactions.ToListAsync();
        }

        public async Task<InventoryTransaction> GetByIdAsync(Guid id)
        {
            return await _context.inventoryTransactions.FindAsync(id);
        }

        public async Task<InventoryTransaction> UpdateAsync(Guid id, InventoryTransaction inventoryTransaction)
        {
            try
            {
                var batch = await _context.inventoryTransactions.FindAsync(id);
                if (batch == null)
                {
                    throw new Exception("Inventory transaction not found.");
                }
                else
                {
                    batch.InventoryBatchId = inventoryTransaction.InventoryBatchId;
                    batch.Quantity = inventoryTransaction.Quantity;
                    batch.CreationTime = inventoryTransaction.CreationTime;
                    batch.CreateBy = inventoryTransaction.CreateBy;
                    batch.Type = inventoryTransaction.Type;
                    batch.Note = inventoryTransaction.Note;
                    _context.inventoryTransactions.Update(batch);
                    await _context.SaveChangesAsync();
                    return batch;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the inventory transaction.", ex);
            }
        }
    }
}
