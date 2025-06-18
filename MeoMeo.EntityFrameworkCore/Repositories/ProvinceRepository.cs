using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProvinceRepository : BaseRepository<Province>, IProvinceRepository
    {
        public ProvinceRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Province> CreateProvinceAsync(Province province)
        {
            var CreateProvice = await AddAsync(province);
            return CreateProvice;
        }

        public async Task<bool> DeleteProvinceAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Province>> GetAllProvinceAsync()
        {
            var province = await GetAllAsync();
            return province;
        }

        public async Task<Province> GetProvinceByIdsync(Guid id)
        {
            var province = await GetByIdAsync(id);
            return province;
        }

        public async Task<Province> UpdateProvinceAsync(Province province)
        {
            var UpdatrProvince = await UpdateAsync(province);
            return UpdatrProvince;
        }
    }
}
