using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class PermissionGroupRepository : BaseRepository<PermissionGroup>, IPermissionGroupRepository
    {
        public PermissionGroupRepository(MeoMeoDbContext context) : base(context)
        {
        }
    }
} 