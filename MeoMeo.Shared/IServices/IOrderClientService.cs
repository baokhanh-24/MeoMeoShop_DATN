using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices;

public interface IOrderClientService
{
    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
        GetListOrderRequestDTO request);
    Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request);
    
    Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId);
}