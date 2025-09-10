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
        Task<BaseResponse> CreatePartialOrderReturnAsync(CreatePartialOrderReturnDTO request, List<ReturnFileUpload> files);
        Task<PagingExtensions.PagedResult<OrderReturnListDTO>?> GetMyOrderReturnsAsync(GetOrderReturnRequestDTO request);
        Task<OrderReturnDetailDTO?> GetOrderReturnByIdAsync(Guid id);
        Task<BaseResponse> CancelOrderReturnAsync(Guid orderReturnId);
        Task<List<OrderReturnItemDetailDTO>?> GetAvailableItemsForReturnAsync(Guid orderId);
        Task<bool> CanOrderBeReturnedAsync(Guid orderId);
        Task<List<OrderReturnListDTO>?> GetByOrderIdAsync(Guid orderId);
    }

    public class ReturnFileUpload
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
    }
}