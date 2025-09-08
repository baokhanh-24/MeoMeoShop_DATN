using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Wishlist;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;

namespace MeoMeoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistController(IWishlistService wishlistService, IHttpContextAccessor httpContextAccessor)
        {
            _wishlistService = wishlistService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("my-wishlist")]
        public async Task<ActionResult<List<WishlistDTO>>> GetMyWishlist()
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty) return Unauthorized();
            var items = await _wishlistService.GetMyWishlistAsync(customerId);
            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<ActionResult<bool>> Add([FromBody] CreateOrUpdateWishlistDTO dto)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty) return Unauthorized();
            var ok = await _wishlistService.AddAsync(customerId, dto.ProductDetailId);
            return Ok(ok);
        }

        [HttpDelete("remove")]
        public async Task<ActionResult<bool>> Remove([FromQuery] Guid productDetailId)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty) return Unauthorized();
            var ok = await _wishlistService.RemoveAsync(customerId, productDetailId);
            return Ok(ok);
        }

        [HttpGet("check/{productDetailId}")]
        public async Task<ActionResult<bool>> Check(Guid productDetailId)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty) return Unauthorized();
            var exists = await _wishlistService.IsInWishlistAsync(customerId, productDetailId);
            return Ok(exists);
        }
    }
}

