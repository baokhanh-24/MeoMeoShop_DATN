using MeoMeo.Contract.DTOs.Wishlist;

namespace MeoMeo.Shared.IServices
{
    public interface IWishlistClientService
    {
        Task<List<WishlistDTO>> GetMyWishlistAsync();
        Task<bool> AddToWishlistAsync(Guid productDetailId);
        Task<bool> RemoveFromWishlistAsync(Guid productDetailId);
        Task<bool> IsInWishlistAsync(Guid productDetailId);
    }
}
