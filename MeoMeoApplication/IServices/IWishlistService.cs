using MeoMeo.Contract.DTOs.Wishlist;

namespace MeoMeo.Application.IServices
{
    public interface IWishlistService
    {
        Task<List<WishlistDTO>> GetMyWishlistAsync(Guid customerId);
        Task<bool> AddAsync(Guid customerId, Guid productDetailId);
        Task<bool> RemoveAsync(Guid customerId, Guid productDetailId);
        Task<bool> IsInWishlistAsync(Guid customerId, Guid productDetailId);
    }
}


