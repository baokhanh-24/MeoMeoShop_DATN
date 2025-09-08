using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Wishlist;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductsDetailRepository _productDetailRepository;

        public WishlistService(IWishlistRepository wishlistRepository, IProductsDetailRepository productDetailRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productDetailRepository = productDetailRepository;
        }

        public async Task<List<WishlistDTO>> GetMyWishlistAsync(Guid customerId)
        {
            var query = _wishlistRepository.Query()
                .Where(w => w.CustomerId == customerId)
                .Select(w => new WishlistDTO
                {
                    CustomerId = w.CustomerId,
                    ProductId = w.ProductDetails.ProductId,
                    ProductDetailId = w.ProductDetailId,
                    CreationTime = w.CreationTime,
                    ProductName = w.ProductDetails.Product.Name,
                    ProductThumbnail = w.ProductDetails.Product.Thumbnail,
                    ProductPrice = w.ProductDetails.Price,
                    ProductStock = w.ProductDetails.InventoryBatches.Sum(b => b.Quantity),
                    IsAvailable = w.ProductDetails.InventoryBatches.Sum(b => b.Quantity) > 0
                });

            return await query.ToListAsync();
        }

        public async Task<bool> AddAsync(Guid customerId, Guid productDetailId)
        {
            // If exists, ignore
            var exists = await _wishlistRepository.Query()
                .AnyAsync(x => x.CustomerId == customerId && x.ProductDetailId == productDetailId);
            if (exists) return true;

            await _wishlistRepository.AddAsync(new Domain.Entities.Wishlist
            {
                CustomerId = customerId,
                ProductDetailId = productDetailId,
                CreationTime = DateTime.UtcNow
            });
            return true;
        }

        public async Task<bool> RemoveAsync(Guid customerId, Guid productDetailId)
        {
            var entity = await _wishlistRepository.Query()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ProductDetailId == productDetailId);
            if (entity == null) return false;
            await _wishlistRepository.DeleteAsync(entity);
            return true;
        }

        public async Task<bool> IsInWishlistAsync(Guid customerId, Guid productDetailId)
        {
            return await _wishlistRepository.Query()
                .AnyAsync(x => x.CustomerId == customerId && x.ProductDetailId == productDetailId);
        }
    }
}


