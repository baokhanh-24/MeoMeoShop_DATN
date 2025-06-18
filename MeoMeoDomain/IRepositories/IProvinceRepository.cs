using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProvinceRepository : IBaseRepository<Province>
    {
        Task<IEnumerable<Province>> GetAllProvinceAsync();
        public Task<Province> GetProvinceByIdsync(Guid id);
        public Task<Province> CreateProvinceAsync(Province province);
        public Task<Province> UpdateProvinceAsync(Province province);
        public Task<bool> DeleteProvinceAsync(Guid id);
    }
}
