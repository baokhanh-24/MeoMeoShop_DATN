using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<CreateOrUpdateOrderResponse> GetOrderByIdAsync(Guid id);
        Task<Order> CreateOrderAsync(CreateOrUpdateOrderDTO order);
        Task<CreateOrUpdateOrderResponse> UpdateOrderAsync(CreateOrUpdateOrderDTO order);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
