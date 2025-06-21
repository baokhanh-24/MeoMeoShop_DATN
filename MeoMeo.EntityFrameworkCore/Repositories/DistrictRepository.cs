using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class DistrictRepository : BaseRepository<District>, IDistrictRepository
    {
        public DistrictRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<District> CreateDistrictAsync(District district)
        {
            var createDistrict = await AddAsync(district);
            return createDistrict;
        }

        public async Task<bool> DeleteDistrictAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<District>> GetAllDistrictAsync()
        {
            var district = await GetAllAsync();
            return district;
        }

        public async Task<District> GetDistrictByIdAsync(Guid id)
        {
            var district = await GetByIdAsync(id);
            return district;
        }

        public async Task<District> UpdateDistrictAsync(District district)
        {
            var updateDistrict = await UpdateAsync(district);
            return updateDistrict;
        }
    }
}
