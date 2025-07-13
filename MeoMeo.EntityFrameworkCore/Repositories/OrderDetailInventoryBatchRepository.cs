using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories;

public class OrderDetailInventoryBatchRepository:BaseRepository<OrderDetailInventoryBatch>, IOrderDetailInventoryBatchRepository
{
    public OrderDetailInventoryBatchRepository(MeoMeoDbContext context) : base(context)
    {
    }
}