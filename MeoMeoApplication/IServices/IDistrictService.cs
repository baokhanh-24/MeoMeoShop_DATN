using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IDistrictService
    {
        Task<IEnumerable<District>> GetAllAsync();
        Task<CreateOrUpdateDistrictRespose> GetDistrictByIdAsync(Guid id);
        Task<District> CreateDistrictAsync(CreateOrUpdateDistrictDTO district);
        Task<CreateOrUpdateDistrictRespose> UpdateDistrictAsync(CreateOrUpdateDistrictDTO district);
        Task<bool> DeleteDistrictAsync(Guid id);
    }
}
