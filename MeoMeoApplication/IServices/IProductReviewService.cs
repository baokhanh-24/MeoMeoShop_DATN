using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Application.IServices
{
    public interface IProductReviewService
    {
        Task<BaseResponse> CreateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload);
        Task<BaseResponse> UpdateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload);
        Task<BaseResponse> DeleteProductReviewAsync(Guid id);
        Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync();
        Task<List<ProductReviewFile>> GetOldFilesAsync(Guid id);
    }
} 