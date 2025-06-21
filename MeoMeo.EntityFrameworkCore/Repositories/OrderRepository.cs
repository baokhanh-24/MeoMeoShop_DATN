using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var createdOrder = await AddAsync(order);
            return createdOrder;
        }

        public async Task<bool> DeleteOrderAsync(Order order)
        {
            await DeleteAsync(order);
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrderAsync()
        {
            var order = await GetAllAsync();
            return order;
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            var order = await GetByIdAsync(id);
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            var updatedOrder = await UpdateAsync(order);
            return updatedOrder;
        }
    }
}
