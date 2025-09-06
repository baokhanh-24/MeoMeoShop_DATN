using MeoMeo.Contract.DTOs.Wishlist;

namespace MeoMeo.Shared.IServices
{
    public interface IWishlistClientService
    {
        Task<List<WishlistDTO>> GetMyWishlistAsync();
        Task<bool> AddToWishlistAsync(Guid productId);
        Task<bool> RemoveFromWishlistAsync(Guid wishlistId);
        Task<bool> IsInWishlistAsync(Guid productId);
    }
}
