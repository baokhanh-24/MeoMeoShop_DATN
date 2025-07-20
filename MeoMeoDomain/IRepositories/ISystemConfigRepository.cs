using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ISystemConfigRepository : IBaseRepository<SystemConfig>
    {
        Task<IEnumerable<SystemConfig>> GetAllSystemConfigsAsync();
        Task<SystemConfig> GetSystemConfigByIdAsync(Guid id);
        Task<SystemConfig> CreateSystemConfigAsync(SystemConfig systemConfig);
        Task<SystemConfig> UpdateSystemConfigAsync(SystemConfig systemConfig);
        Task<bool> DeleteSystemConfigAsync(Guid id);
    }
}
