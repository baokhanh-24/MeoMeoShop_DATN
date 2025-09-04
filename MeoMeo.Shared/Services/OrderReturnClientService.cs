using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services;

public class OrderReturnClientService : IOrderReturnClientService
{
    private readonly IApiCaller _httpClient;
    private readonly ILogger<OrderReturnClientService> _logger;

    public OrderReturnClientService(IApiCaller httpClient, ILogger<OrderReturnClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CreateOrderReturnResponseDTO> CreateAsync(CreateOrderReturnRequestDTO request)
    {
        try
        {
            var url = "/api/OrderReturns";
            var result = await _httpClient.PostAsync<CreateOrderReturnRequestDTO, CreateOrderReturnResponseDTO>(url, request);
            return result ?? new CreateOrderReturnResponseDTO
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Không thể tạo yêu cầu hoàn hàng"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi tạo yêu cầu hoàn hàng: {Message}", ex.Message);
            return new CreateOrderReturnResponseDTO
            {
                ResponseStatus = BaseStatus.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<UpdateOrderReturnStatusResponseDTO> UpdateStatusAsync(UpdateOrderReturnStatusRequestDTO request)
    {
        try
        {
            var url = "/api/OrderReturns/status";
            var result = await _httpClient.PutAsync<UpdateOrderReturnStatusRequestDTO, UpdateOrderReturnStatusResponseDTO>(url, request);
            return result ?? new UpdateOrderReturnStatusResponseDTO
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Không thể cập nhật trạng thái"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật trạng thái hoàn hàng: {Message}", ex.Message);
            return new UpdateOrderReturnStatusResponseDTO
            {
                ResponseStatus = BaseStatus.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<OrderReturnViewDTO?> GetByIdAsync(Guid id)
    {
        try
        {
            var url = $"/api/OrderReturns/{id}";
            var result = await _httpClient.GetAsync<OrderReturnViewDTO>(url);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi lấy thông tin hoàn hàng {Id}: {Message}", id, ex.Message);
            return null;
        }
    }

    public async Task<List<OrderReturnViewDTO>> GetByOrderIdAsync(Guid orderId)
    {
        try
        {
            var url = $"/api/OrderReturns/order/{orderId}";
            var result = await _httpClient.GetAsync<List<OrderReturnViewDTO>>(url);
            return result ?? new List<OrderReturnViewDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách hoàn hàng cho đơn hàng {OrderId}: {Message}", orderId, ex.Message);
            return new List<OrderReturnViewDTO>();
        }
    }
}
