using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            var createOrderDetail = await AddAsync(orderDetail);
            return createOrderDetail;
        }

        public async Task<bool> DeleteOrderDetailAsync(OrderDetail orderDetail)
        {
            await DeleteAsync(orderDetail);
            return true;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailAsync()
        {
            var orderDetail = await GetAllAsync();
            return orderDetail;
        }

        public async Task<OrderDetail> GetOrderDetailByIdAsync(Guid id)
        {
            var orderDetail = await GetByIdAsync(id);
            return orderDetail;
        }

        public async Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            var UpdatedOrderDetail = await UpdateAsync(orderDetail);
            return UpdatedOrderDetail;
        }
    }
}
