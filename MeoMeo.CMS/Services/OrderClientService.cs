using MeoMeo.CMS.IServices;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using MeoMeo.Utilities;

namespace MeoMeo.CMS.Services;

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
}