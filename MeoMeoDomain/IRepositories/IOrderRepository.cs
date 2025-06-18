using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;


namespace MeoMeo.Domain.IRepositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllAsync();
        public Task<Order> GetOrderByIdAsync(Guid id);
        public Task<Order> CreateOrderAsync(Order order);
        public Task<Order> UpdateOrderAsync(Order order);
        public Task<bool> DeleteOrderAsync(Order order);
    }
}
