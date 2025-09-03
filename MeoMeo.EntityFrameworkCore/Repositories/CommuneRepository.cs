using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class CommuneRepository : BaseRepository<Commune>, ICommuneRepository
    {
        public CommuneRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Commune> CreateAsync(Commune commune)
        {
            var result = await AddAsync(commune);
            return result;
        }

        public async Task DeleteAsync(Commune commune)
        {
            await DeleteAsync(commune);
        }

        public async Task<Commune> GetByIdAsync(Guid id)
        {
            var commune = await GetByIdAsync(id);
            return commune;
        }

        public async Task<Commune> UpdateAsync(Commune commune)
        {
            var result = await UpdateAsync(commune);
            return result;
        }
    }
}
