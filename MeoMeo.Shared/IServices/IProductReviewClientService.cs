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
        Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetByProductDetailIdsAsync(GetListProductReviewDTO request);
        
        // New methods
        Task<List<OrderItemForReviewDTO>> GetUnreviewedOrderItemsAsync();
        Task<List<ProductReviewDTO>> GetMyReviewsAsync();
    }
}


