using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProductReviewRepository : BaseRepository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<ProductReview> CreateProductReviewAsync(ProductReview review)
        {
            return await AddAsync(review);
        }

        public async Task<ProductReview> UpdateProductReviewAsync(ProductReview review)
        {
            return await UpdateAsync(review);
        }

        public async Task<bool> DeleteProductReviewAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<ProductReview> GetProductReviewByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync()
        {
            var items = await GetAllAsync();
            return items.ToList();
        }

        
    }
}