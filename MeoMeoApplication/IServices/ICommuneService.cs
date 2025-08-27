using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface ICommuneService
    {
        Task<IEnumerable<Commune>> GetAllAsync();
        Task<Commune> GetByIdAsync(Guid id);
        Task<IEnumerable<Commune>> GetByDistrictIdAsync(Guid districtId);
        Task<Commune> CreateAsync(CreateOrUpdateCommuneDTO commune);
        Task<Commune> UpdateAsync(CreateOrUpdateCommuneDTO commune);
        Task<bool> DeleteAsync(Guid id);
    }
}
