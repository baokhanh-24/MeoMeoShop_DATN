using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetail>> GetAllDetailAsync();
        Task<OrderDetail> GetOrderDetailByIdAsync(Guid id);
        Task<OrderDetail> CreateOrderDetailAsync(CreateOrUpdateOrderDetailDTO order);
        Task<OrderDetail> UpdateOrderDetailAsync(CreateOrUpdateOrderDetailDTO order);
        Task<bool> DeleteOrderOrderDetailAsync(Guid id);
    }
}
