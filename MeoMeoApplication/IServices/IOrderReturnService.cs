using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IOrderReturnService
    {
        Task<BaseResponse> CreatePartialOrderReturnAsync(Guid customerId, CreatePartialOrderReturnDTO request);
        Task<PagingExtensions.PagedResult<OrderReturnListDTO>> GetOrderReturnsAsync(GetOrderReturnRequestDTO request);
        Task<PagingExtensions.PagedResult<OrderReturnListDTO>> GetMyOrderReturnsAsync(Guid customerId, GetOrderReturnRequestDTO request);
        Task<OrderReturnDetailDTO?> GetOrderReturnByIdAsync(Guid id);
        Task<BaseResponse> UpdateOrderReturnStatusAsync(Guid id, UpdateOrderReturnStatusRequestDTO request);
        Task<BaseResponse> UpdatePayBackAmountAsync(Guid id, UpdatePayBackAmountDTO request);
        Task<BaseResponse> CancelOrderReturnAsync(Guid customerId, Guid orderReturnId);
        Task<List<OrderReturnItemDetailDTO>> GetAvailableItemsForReturnAsync(Guid orderId);
        Task<bool> CanOrderBeReturnedAsync(Guid orderId);
    }
}