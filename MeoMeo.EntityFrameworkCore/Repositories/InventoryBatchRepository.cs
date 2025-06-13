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
    public class InventoryBatchRepository : IIventoryBtachReposiory
    {
        private readonly MeoMeoDbContext _context;
        public InventoryBatchRepository(MeoMeoDbContext iventoryBtachReposiory)
        {
            _context = iventoryBtachReposiory;
        }
        public async Task<InventoryBatch> CreateAsync(InventoryBatch inventoryBatch)
        {
            try
            {
                _context.inventoryBatches.Add(inventoryBatch);
                await _context.SaveChangesAsync();
                return inventoryBatch;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the inventory batch.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var batch = await _context.inventoryBatches.FindAsync(id);
                if(batch != null)
                {
                    _context.inventoryBatches.Remove(batch);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the inventory batch.", ex);
            }
        }

        public async Task<List<InventoryBatch>> GetAllAsync()
        {
            return await _context.inventoryBatches.ToListAsync();
        }

        public async Task<InventoryBatch> GetByIdAsync(Guid id)
        {
            return await _context.inventoryBatches.FindAsync(id);
        }

        public async Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatch inventoryBatch)
        {
            try
            {
                var batch = await _context.inventoryBatches.FindAsync(id);
                if(batch == null)
                {
                    throw new Exception("Inventory batch not found.");
                }
                else
                {
                    batch.ProductDetailId = inventoryBatch.ProductDetailId;
                    batch.OriginalPrice = inventoryBatch.OriginalPrice;
                    batch.Code = inventoryBatch.Code;
                    batch.Quantity = inventoryBatch.Quantity;
                    batch.Note = inventoryBatch.Note;
                    batch.Status = inventoryBatch.Status;
                    _context.inventoryBatches.Update(batch);
                    await _context.SaveChangesAsync();
                    return batch;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
