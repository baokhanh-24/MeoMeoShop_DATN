using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailAsync();
        public Task<OrderDetail> GetOrderDetailByIdAsync(Guid id);
        public Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail);
        public Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail);
        public Task<bool> DeleteOrderDetailAsync(OrderDetail orderDetail);
    }
}
