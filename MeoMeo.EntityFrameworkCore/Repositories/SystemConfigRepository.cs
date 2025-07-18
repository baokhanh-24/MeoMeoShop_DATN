using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class SystemConfigRepository : BaseRepository<SystemConfig>, ISystemConfigRepository
    {
        public SystemConfigRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<SystemConfig> CreateSystemConfigAsync(SystemConfig systemConfig)
        {
            var addedConfig = await AddAsync(systemConfig);
            return addedConfig;
        }

        public async Task<bool> DeleteSystemConfigAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<SystemConfig>> GetAllSystemConfigsAsync()
        {
            var getallSystemConfigs = await GetAllAsync();
            return getallSystemConfigs.ToList();
        }

        public async Task<SystemConfig> GetSystemConfigByIdAsync(Guid id)
        {
            var systemConfig = await GetByIdAsync(id);
            return systemConfig;
        }

        public async Task<SystemConfig> UpdateSystemConfigAsync(SystemConfig systemConfig)
        {
            var updatedConfig = await UpdateAsync(systemConfig);
            return updatedConfig;
        }
    }
}
