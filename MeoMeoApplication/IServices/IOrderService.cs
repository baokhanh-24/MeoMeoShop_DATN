using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IOrderService
    {
        Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
            GetListOrderRequestDTO request);
        Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderByCustomerAsync(
            GetListOrderRequestDTO request, Guid customerId);
        Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request);
        Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId);
        Task<CreateOrderResultDTO> CreateOrderAsync(Guid customerId, Guid userId, CreateOrderDTO request);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<CreatePosOrderResultDTO> CreatePosOrderAsync(CreatePosOrderDTO request);
    }
}
