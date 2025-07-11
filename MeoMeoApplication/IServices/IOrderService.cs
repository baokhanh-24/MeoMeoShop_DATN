using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IOrderService
    {
        Task<PagingExtensions.PagedResult<OrderDTO>> GetListOrderAsync(GetListOrderRequestDTO  request);
        Task<UpdateStatusOrderResponseDTO> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request);
        Task<BaseResponse> CreateOrderAsync(CreateOrderDTO request);
        // Task<Order> GetOrderByIdAsync(Guid id);
        // Task<Order> CreateOrderAsync(CreateOrUpdateOrderDTO order);
        // Task<CreateOrUpdateOrderResponse> UpdateOrderAsync(CreateOrUpdateOrderDTO order);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
