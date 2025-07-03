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
            var addInventoryBatch = await AddAsync(inventoryBatch);
            return addInventoryBatch;
        }

        public async Task<bool> DeleteInventoryBatchAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }


        public async Task<InventoryBatch> GetBatchByIdAsync(Guid id)
        {
            var batch = await GetByIdAsync(id);
            return batch;
        }

        public async Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatch inventoryBatch)
        {
            var Update = await UpdateAsync(id, inventoryBatch);
            return Update;
        }

        public async Task<List<InventoryBatch>> GetAllBatchAsync()
        {
            var getAll = await GetAllAsync();
            return getAll.ToList();
        }
    }
}
