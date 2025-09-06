using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using MeoMeo.Domain.Entities;
using MeoMeo.Contract.DTOs.Wishlist;
using System.Security.Claims;

namespace MeoMeoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Customer")]
    public class WishlistController : ControllerBase
    {
        private readonly MeoMeoDbContext _db;

        public WishlistController(MeoMeoDbContext db)
        {
            _db = db;
        }

        [HttpGet("my-wishlist")]
        public async Task<ActionResult<List<WishlistDTO>>> GetMyWishlist()
        {
            var customerIdStr = User.FindFirstValue("CustomerId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(customerIdStr, out var customerId)) return Unauthorized();

            var items = await _db.wishlists
                .Where(w => w.CustomerId == customerId)
                .Include(w => w.Product)
                .Select(w => new WishlistDTO
                {
                    Id = w.Id,
                    CustomerId = w.CustomerId,
                    ProductId = w.ProductId,
                    CreationTime = w.CreationTime,
                    ProductName = w.Product.Name,
                    ProductThumbnail = w.Product.Thumbnail ?? string.Empty,
                    ProductPrice = w.Product.ProductDetails.Select(pd => (decimal?)pd.Price).Min() ?? 0,
                    // ProductStock = w.Product.ProductDetails.Sum(pd => pd.InventoryQuantity),
                    // IsAvailable = w.Product.ProductDetails.Sum(pd => pd.InventoryQuantity) > 0
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<ActionResult<bool>> Add([FromBody] CreateOrUpdateWishlistDTO dto)
        {
            var customerIdStr = User.FindFirstValue("CustomerId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(customerIdStr, out var customerId)) return Unauthorized();

            var exists = await _db.wishlists.AnyAsync(w => w.CustomerId == customerId && w.ProductId == dto.ProductId);
            if (exists) return Ok(true);

            var entity = new Wishlist
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = dto.ProductId,
                CreationTime = DateTime.UtcNow
            };
            _db.wishlists.Add(entity);
            await _db.SaveChangesAsync();
            return Ok(true);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var customerIdStr = User.FindFirstValue("CustomerId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(customerIdStr, out var customerId)) return Unauthorized();

            var entity = await _db.wishlists.FirstOrDefaultAsync(w => w.Id == id && w.CustomerId == customerId);
            if (entity == null) return NotFound(false);
            _db.wishlists.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok(true);
        }

        [HttpGet("check/{productId}")]
        public async Task<ActionResult<bool>> Check(Guid productId)
        {
            var customerIdStr = User.FindFirstValue("CustomerId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(customerIdStr, out var customerId)) return Unauthorized();

            var exists = await _db.wishlists.AnyAsync(w => w.CustomerId == customerId && w.ProductId == productId);
            return Ok(exists);
        }
    }
}


