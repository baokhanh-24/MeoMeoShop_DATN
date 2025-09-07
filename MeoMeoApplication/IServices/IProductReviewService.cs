using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Application.IServices
{
    public interface IProductReviewService
    {
        Task<BaseResponse> CreateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload);
        Task<BaseResponse> UpdateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload);
        Task<BaseResponse> DeleteProductReviewAsync(Guid id);
        Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync();
        Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetProductReviewsByProductDetailIdAsync(GetListProductReviewDTO request);
        Task<List<ProductReviewFile>> GetOldFilesAsync(Guid id);
        
        // New methods for customer review management
        Task<List<OrderItemForReviewDTO>> GetUnreviewedOrderItemsAsync(Guid customerId);
        Task<List<ProductReviewDTO>> GetCustomerReviewsAsync(Guid customerId);
    }
}