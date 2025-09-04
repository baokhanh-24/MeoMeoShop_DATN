using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class OrderReturnFileRepository : BaseRepository<OrderReturnFile>, IOrderReturnFileRepository
    {
        public OrderReturnFileRepository(MeoMeoDbContext context) : base(context)
        {
        }
    }
}


