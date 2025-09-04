using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class OrderReturnItemRepository : BaseRepository<OrderReturnItem>, IOrderReturnItemRepository
    {
        public OrderReturnItemRepository(MeoMeoDbContext context) : base(context)
        {
        }
    }
}


