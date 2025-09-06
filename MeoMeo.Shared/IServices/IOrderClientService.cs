using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices;

public interface IOrderClientService
{
    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
        GetListOrderRequestDTO request);
    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetMyOrdersAsync(
        GetListOrderRequestDTO request);
    Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request);
    Task<BaseResponse> CancelOrderAsync(Guid orderId);
    Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId);
    Task<string> CreateVnpayPaymentUrlAsync(CreatePaymentUrlDTO request);
    Task<CreatePosOrderResultDTO> CreatePosOrderAsync(CreatePosOrderDTO request);
}