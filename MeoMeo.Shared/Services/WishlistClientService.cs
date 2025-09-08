using MeoMeo.Contract.DTOs.Wishlist;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services
{
    public class WishlistClientService : IWishlistClientService
    {
        private readonly IApiCaller _apiCaller;

        public WishlistClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<List<WishlistDTO>> GetMyWishlistAsync()
        {
            try
            {
                var response = await _apiCaller.GetAsync<List<WishlistDTO>>("api/wishlist/my-wishlist");
                return response ?? new List<WishlistDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting wishlist: {ex.Message}");
                return new List<WishlistDTO>();
            }
        }

        public async Task<bool> AddToWishlistAsync(Guid productDetailId)
        {
            try
            {
                var request = new CreateOrUpdateWishlistDTO
                {
                    ProductDetailId = productDetailId
                };

                var response = await _apiCaller.PostAsync<CreateOrUpdateWishlistDTO, bool>("api/wishlist/add", request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to wishlist: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(Guid productDetailId)
        {
            try
            {
                var response = await _apiCaller.DeleteAsync($"api/wishlist/remove?productDetailId={productDetailId}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing from wishlist: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsInWishlistAsync(Guid productDetailId)
        {
            try
            {
                var response = await _apiCaller.GetAsync<bool>($"api/wishlist/check/{productDetailId}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking wishlist: {ex.Message}");
                return false;
            }
        }
    }
}
