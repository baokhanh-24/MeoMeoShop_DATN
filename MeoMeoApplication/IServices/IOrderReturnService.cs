using MeoMeo.Contract.DTOs.Order.Return;

namespace MeoMeo.Application.IServices
{
    public interface IOrderReturnService
    {
        Task<CreateOrderReturnResponseDTO> CreateAsync(CreateOrderReturnRequestDTO request, Guid currentCustomerId);
        Task<UpdateOrderReturnStatusResponseDTO> UpdateStatusAsync(UpdateOrderReturnStatusRequestDTO request);
        Task<OrderReturnViewDTO?> GetByIdAsync(Guid id);
        Task<List<OrderReturnViewDTO>> GetByOrderIdAsync(Guid orderId);
    }
}


