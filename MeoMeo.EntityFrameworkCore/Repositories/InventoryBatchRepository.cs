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
    public class InventoryBatchRepository : BaseRepository<InventoryBatch>, IIventoryBtachReposiory
    {
        public InventoryBatchRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<InventoryBatch> CreateAsync(InventoryBatch inventoryBatch)
        {
            try
            {
                await AddAsync(inventoryBatch);
                return inventoryBatch;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the inventory batch.", ex);
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


        public async Task<InventoryBatch> GetBatchByIdAsync(Guid id)
        {
            var batch = await GetByIdAsync(id);
            return batch;
        }

        public async Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatch inventoryBatch)
        {
            var phat = await GetByIdAsync(id);
            if (phat == null)
            {
                throw new Exception("Inventory batch not found.");
            }
            phat.ProductDetailId = inventoryBatch.ProductDetailId;
            phat.OriginalPrice = inventoryBatch.OriginalPrice;
            phat.Code = inventoryBatch.Code;
            phat.Quantity = inventoryBatch.Quantity;
            phat.Note = inventoryBatch.Note;
            phat.Status = inventoryBatch.Status;
            await UpdateAsync(phat);
            return phat;
        }

        async Task<IEnumerable<InventoryBatch>> IIventoryBtachReposiory.GetAllBatchAsync()
        {
            return await GetAllAsync();
        }
    }
}
