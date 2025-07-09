using MeoMeo.CMS.IServices;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using MeoMeo.Utilities;

namespace MeoMeo.CMS.Services;

public class OrderClientService:IOrderClientService
{
    private readonly IApiCaller _httpClient;
    private readonly ILogger<MaterialClientService> _logger;

    public OrderClientService(IApiCaller httpClient, ILogger<MaterialClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<PagingExtensions.PagedResult<OrderDTO>> GetListOrderAsync(GetListOrderRequestDTO filter)
    {
        var query = BuildQuery.ToQueryString(filter);
        var url = $"/api/Orders/get-list-order-async?{query}";
        var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<OrderDTO>>(url);
        return response ?? new PagingExtensions.PagedResult<OrderDTO>();
    }
}