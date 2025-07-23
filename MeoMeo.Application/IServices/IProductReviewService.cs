using MeoMeo.Contract.Commons;

public interface IProductReviewService
{
    Task<ProductReview> CreateProductReviewAsync(ProductReviewCreateDTO dto);
    Task<ProductReview> UpdateProductReviewAsync(Guid id, ProductReviewCreateDTO dto, List<FileUploadResult> filesUpload);
    Task<bool> DeleteProductReviewAsync(Guid id);
    Task<ProductReview> GetProductReviewByIdAsync(Guid id);
    Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync();
    Task<List<ProductReviewFile>> GetOldFilesAsync(Guid reviewId);
} 