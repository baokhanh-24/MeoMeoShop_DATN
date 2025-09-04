using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;

namespace MeoMeo.Shared.IServices;

public interface IOrderReturnClientService
{
    Task<CreateOrderReturnResponseDTO> CreateAsync(CreateOrderReturnRequestDTO request);
    Task<UpdateOrderReturnStatusResponseDTO> UpdateStatusAsync(UpdateOrderReturnStatusRequestDTO request);
    Task<OrderReturnViewDTO?> GetByIdAsync(Guid id);
    Task<List<OrderReturnViewDTO>> GetByOrderIdAsync(Guid orderId);
}
