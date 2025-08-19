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
        Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request);
        Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId);
        Task<CreateOrderResultDTO> CreateOrderAsync(CreateOrderDTO request);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
