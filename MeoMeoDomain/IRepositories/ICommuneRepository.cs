using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICommuneRepository : IBaseRepository<Commune>
    {
        Task<Commune> GetByIdAsync(Guid id);
        Task<Commune> CreateAsync(Commune commune);
        Task<Commune> UpdateAsync(Commune commune);
        Task DeleteAsync(Commune commune);
    }
}
