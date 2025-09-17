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
    Task<BaseResponse> CancelOrderWithReasonAsync(Guid orderId, string reason);
    Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId);
    Task<string> CreateVnpayPaymentUrlAsync(CreatePaymentUrlDTO request);
    Task<CreatePosOrderResultDTO> CreatePosOrderAsync(CreatePosOrderDTO request);

    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
        GetListOrderRequestDTO filter);

    // Pending Orders methods
    Task<GetPendingOrdersResponseDTO?> GetPendingOrdersAsync(GetPendingOrdersRequestDTO request);
    Task<BaseResponse> DeletePendingOrderAsync(Guid orderId);
    Task<CreatePosOrderResultDTO?> UpdatePosOrderAsync(Guid orderId, CreatePosOrderDTO request);
}