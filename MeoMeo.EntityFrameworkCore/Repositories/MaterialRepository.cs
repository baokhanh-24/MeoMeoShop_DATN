using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class MaterialRepository : BaseRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Material> CreateMaterialAsync(Material material)
        {
            var addedMaterial = await AddAsync(material);
            return addedMaterial;
        }

        public async Task<bool> DeleteMaterialAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Material>> GetAllMaterialsAsync()
        {
            var getallMaterials = await GetAllAsync();
            return getallMaterials.ToList();

        }

        public async Task<Material> GetMaterialByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Material> UpdateMaterialAsync(Material material)
        {
            var updateMaterial = await UpdateAsync(material);
            return updateMaterial;
        }
    }
}
