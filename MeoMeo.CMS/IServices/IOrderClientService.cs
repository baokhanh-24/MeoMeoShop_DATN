using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;

namespace MeoMeo.CMS.IServices;

public interface IOrderClientService
{
    Task<PagingExtensions.PagedResult<OrderDTO>> GetListOrderAsync(GetListOrderRequestDTO  filter);
}