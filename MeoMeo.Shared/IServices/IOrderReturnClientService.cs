using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Shared.IServices
{
    public interface IOrderReturnClientService
    {
        Task<BaseResponse> CreatePartialOrderReturnAsync(CreatePartialOrderReturnDTO request);
        Task<PagingExtensions.PagedResult<OrderReturnListDTO>?> GetMyOrderReturnsAsync(GetOrderReturnRequestDTO request);
        Task<OrderReturnDetailDTO?> GetOrderReturnByIdAsync(Guid id);
        Task<BaseResponse> CancelOrderReturnAsync(Guid orderReturnId);
        Task<List<OrderReturnItemDetailDTO>?> GetAvailableItemsForReturnAsync(Guid orderId);
        Task<bool> CanOrderBeReturnedAsync(Guid orderId);
        Task<List<OrderReturnListDTO>?> GetByOrderIdAsync(Guid orderId);
    }
}