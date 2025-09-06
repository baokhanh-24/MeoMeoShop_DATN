using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductReviewRepository : IBaseRepository<ProductReview>
    {
        Task<ProductReview> CreateProductReviewAsync(ProductReview review);
        Task<ProductReview> UpdateProductReviewAsync(ProductReview review);
        Task<bool> DeleteProductReviewAsync(Guid id);
        Task<ProductReview> GetProductReviewByIdAsync(Guid id);
        Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync();
    }
}