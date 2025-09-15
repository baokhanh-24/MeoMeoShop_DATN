using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices;

public interface IOrderClientService
{
    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetOrdersByCustomerIdAsync(
        GetListOrderRequestDTO request, Guid customerId);
    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetMyOrdersAsync(
        GetListOrderRequestDTO request);
    Task<OrderDTO?> GetOrderByIdAsync(Guid orderId);
    Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request);
    Task<BaseResponse> CancelOrderAsync(Guid orderId);
    Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId);
    Task<string> CreateVnpayPaymentUrlAsync(CreatePaymentUrlDTO request);
    Task<CreatePosOrderResultDTO> CreatePosOrderAsync(CreatePosOrderDTO request);

    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
        GetListOrderRequestDTO filter);
}