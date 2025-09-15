using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IProductReviewClientService
    {
        Task<BaseResponse> CreateAsync(ProductReviewCreateOrUpdateDTO dto);
        Task<BaseResponse> UpdateAsync(ProductReviewCreateOrUpdateDTO dto);
        Task<BaseResponse> DeleteAsync(Guid id);
        Task<List<ProductReviewDTO>> GetAllAsync();

        Task<PagingExtensions.PagedResult<ProductReviewDTO>>
            GetByProductDetailIdsAsync(GetListProductReviewDTO request);

        // New methods
        Task<List<OrderItemForReviewDTO>> GetUnreviewedOrderItemsAsync();
        Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetMyReviewsAsync(GetListMyReviewedDTO request);

        // Admin methods
        Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetAllReviewsForAdminAsync(GetListProductReviewForAdminDTO request);
        Task<BaseResponse> ReplyToReviewAsync(Guid reviewId, string answer);
        Task<BaseResponse> ToggleReviewVisibilityAsync(Guid reviewId, string? reason = null);
        Task<BaseResponse> SetReviewVisibilityAsync(Guid reviewId, bool isHidden, string? reason = null);
        Task<List<ProductReviewDTO>> GetReviewsByOrderIdAsync(Guid orderId);
    }
}


