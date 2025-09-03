using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services;

public class OrderClientService:IOrderClientService
{
    private readonly IApiCaller _httpClient;
    private readonly ILogger<OrderClientService> _logger;

    public OrderClientService(IApiCaller httpClient, ILogger<OrderClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<PagingExtensions.PagedResult<OrderDTO,GetListOrderResponseDTO>> GetListOrderAsync(GetListOrderRequestDTO filter)
    {
        var query = BuildQuery.ToQueryString(filter);
        var url = $"/api/Orders/get-list-order-async?{query}";
        var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<OrderDTO,GetListOrderResponseDTO>>(url);
        return response ?? new PagingExtensions.PagedResult<OrderDTO,GetListOrderResponseDTO>();
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
            _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật trạng thái Order {Id}: {Message}", string.Join(',',request.OrderIds), ex.Message);
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
}