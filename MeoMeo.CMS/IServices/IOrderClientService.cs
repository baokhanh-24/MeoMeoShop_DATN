using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.CMS.IServices;

public interface IOrderClientService
{
    Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
        GetListOrderRequestDTO request);
    Task<BaseResponse> UpdateStatusOrderAsync([FromBody] UpdateStatusOrderRequestDTO request);
}