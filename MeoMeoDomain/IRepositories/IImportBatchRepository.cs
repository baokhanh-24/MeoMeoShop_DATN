using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IImportBatchRepository : IBaseRepository<ImportBatch>
    {
        Task<ImportBatch?> GetImportBatchByCodeAsync(string code);
        Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null);
        Task<List<ImportBatch>> GetImportBatchesWithInventoryBatchesAsync();
        Task<ImportBatch?> GetImportBatchWithDetailsAsync(Guid id);
    }
}
