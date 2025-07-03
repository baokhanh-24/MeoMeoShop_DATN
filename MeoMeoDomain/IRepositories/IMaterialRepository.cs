using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IMaterialRepository : IBaseRepository<Material>
    {
        Task<List<Material>> GetAllMaterialsAsync();
        Task<Material> GetMaterialByIdAsync(Guid id);
        Task<Material> CreateMaterialAsync(Material material);
        Task<Material> UpdateMaterialAsync(Material material);
        Task<bool> DeleteMaterialAsync(Guid id);
    }
}
