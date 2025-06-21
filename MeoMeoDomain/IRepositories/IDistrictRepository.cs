using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IDistrictRepository : IBaseRepository<District>
    {
        Task<IEnumerable<District>> GetAllDistrictAsync();
        public Task<District> GetDistrictByIdAsync(Guid id);
        public Task<District> CreateDistrictAsync(District district);
        public Task<District> UpdateDistrictAsync(District district);
        public Task<bool> DeleteDistrictAsync(Guid id);
    }
}
