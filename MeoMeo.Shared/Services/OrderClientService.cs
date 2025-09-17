using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services;

public class OrderClientService : IOrderClientService
{
    private readonly IApiCaller _httpClient;
    private readonly ILogger<OrderClientService> _logger;

    public OrderClientService(IApiCaller httpClient, ILogger<OrderClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetOrdersByCustomerIdAsync(
        GetListOrderRequestDTO request, Guid customerId)
    {
        try
        {
            var query = BuildQuery.ToQueryString(request);
            var url = $"/api/Orders/get-orders-by-customer/{customerId}?{query}";
            var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>>(url);
            return response ?? new PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi lấy đơn hàng của khách hàng {CustomerId}: {Message}", customerId, ex.Message);
            return new PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>();
        }
    }

    public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(GetListOrderRequestDTO filter)
    {
        var query = BuildQuery.ToQueryString(filter);
        var url = $"/api/Orders/get-list-order-async?{query}";
        var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>>(url);
        return response ?? new PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>();
    }

    public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetMyOrdersAsync(GetListOrderRequestDTO filter)
    {
        var query = BuildQuery.ToQueryString(filter);
        var url = $"/api/Orders/get-my-orders?{query}";
        var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>>(url);
        return response ?? new PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>();
    }

    public async Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request)
    {
        try
        {
            var url = $"/api/Orders/update-status-order-async";
            var result = await _httpClient.PutAsync<UpdateStatusOrderRequestDTO, BaseResponse>(url, request);
            return result ?? new BaseResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật trạng thái Order {Id}: {Message}", string.Join(',', request.OrderIds), ex.Message);
            return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
        }
    }

    public async Task<BaseResponse> CancelOrderAsync(Guid orderId)
    {
        try
        {
            var url = $"/api/Orders/cancel-order/{orderId}";
            var result = await _httpClient.PutAsync<object, BaseResponse>(url, new { });
            return result ?? new BaseResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Không thể hủy đơn hàng"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi hủy Order {Id}: {Message}", orderId, ex.Message);
            return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
        }
    }

    public async Task<BaseResponse> CancelOrderWithReasonAsync(Guid orderId, string reason)
    {
        try
        {
            var url = $"/api/Orders/cancel-order-with-reason/{orderId}";
            var request = new { Reason = reason };
            var result = await _httpClient.PutAsync<object, BaseResponse>(url, request);
            return result ?? new BaseResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Không thể hủy đơn hàng"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi hủy Order {Id} với lý do: {Message}", orderId, ex.Message);
            return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
        }
    }

    public async Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId)
    {
        var url = $"/api/Orders/history/{orderId}";
        var response = await _httpClient.GetAsync<GetListOrderHistoryResponseDTO>(url);
        return response ?? new GetListOrderHistoryResponseDTO();
    }

    public async Task<string> CreateVnpayPaymentUrlAsync(CreatePaymentUrlDTO request)
    {
        var url = "/api/Orders/take-vn-pay";
        var response = await _httpClient.PostAsync<CreatePaymentUrlDTO, string>(url, request);
        return response ?? string.Empty;
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            var url = $"/api/Orders/{orderId}";
            var response = await _httpClient.GetAsync<OrderDTO>(url);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order by id {OrderId}: {Message}", orderId, ex.Message);
            return null;
        }
    }

    public async Task<CreatePosOrderResultDTO> CreatePosOrderAsync(CreatePosOrderDTO request)
    {
        var url = "/api/Orders/create-pos-order";
        var response = await _httpClient.PostAsync<CreatePosOrderDTO, CreatePosOrderResultDTO>(url, request);
        return response ?? new CreatePosOrderResultDTO { ResponseStatus = BaseStatus.Error, Message = "Không tạo được đơn POS" };
    }

    // Pending Orders methods
    public async Task<GetPendingOrdersResponseDTO?> GetPendingOrdersAsync(GetPendingOrdersRequestDTO request)
    {
        try
        {
            var query = BuildQuery.ToQueryString(request);
            var url = $"/api/Orders/get-pending-orders?{query}";
            var response = await _httpClient.GetAsync<GetPendingOrdersResponseDTO>(url);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending orders: {Message}", ex.Message);
            return null;
        }
    }

    public async Task<BaseResponse> DeletePendingOrderAsync(Guid orderId)
    {
        try
        {
            var url = $"/api/Orders/delete-pending-order/{orderId}";
            var response = await _httpClient.DeleteAsync(url);
            return new BaseResponse { ResponseStatus = response ? BaseStatus.Success : BaseStatus.Error, Message = response ? "" : "Không thể xóa đơn hàng" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pending order {OrderId}: {Message}", orderId, ex.Message);
            return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = $"Lỗi khi xóa đơn hàng: {ex.Message}" };
        }
    }

    public async Task<CreatePosOrderResultDTO?> UpdatePosOrderAsync(Guid orderId, CreatePosOrderDTO request)
    {
        try
        {
            var url = $"/api/Orders/update-pos-order/{orderId}";
            var response = await _httpClient.PutAsync<CreatePosOrderDTO, CreatePosOrderResultDTO>(url, request);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating POS order {OrderId}: {Message}", orderId, ex.Message);
            return new CreatePosOrderResultDTO
            {
                ResponseStatus = BaseStatus.Error,
                Message = $"Lỗi khi cập nhật đơn hàng: {ex.Message}"
            };
        }
    }
}