using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<Order> CreateOrderAsync(CreateOrUpdateOrderDTO order);
        Task<Order> UpdateOrderAsync(CreateOrUpdateOrderDTO order);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
