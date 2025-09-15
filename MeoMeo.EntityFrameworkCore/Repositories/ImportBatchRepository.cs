using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ImportBatchRepository : BaseRepository<ImportBatch>, IImportBatchRepository
    {
        public ImportBatchRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<ImportBatch?> GetImportBatchByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Code == code);
        }
        
        public async Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _dbSet.Where(x => x.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<List<ImportBatch>> GetImportBatchesWithInventoryBatchesAsync()
        {
            return await _dbSet
                .Include(x => x.InventoryBatches)
                .ThenInclude(x => x.ProductDetail)
                .ThenInclude(x => x.Product)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
        }

        public async Task<ImportBatch?> GetImportBatchWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.InventoryBatches)
                .ThenInclude(x => x.ProductDetail)
                .ThenInclude(x => x.Product)
                .Include(x => x.InventoryBatches)
                .ThenInclude(x => x.ProductDetail)
                .ThenInclude(x => x.Colour)
                .Include(x => x.InventoryBatches)
                .ThenInclude(x => x.ProductDetail)
                .ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
