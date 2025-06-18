using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class CartDetaillRepository : BaseRepository<CartDetail>, ICartDetaillRepository
    {
        public CartDetaillRepository(MeoMeoDbContext context) : base(context)
        {
            
        }
        public async Task<CartDetail> Create(CartDetail cartDetails)
        {
            bool exists = await _context.cartDetails.AnyAsync(c => c.Id == cartDetails.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await AddAsync(cartDetails);
            await SaveChangesAsync();
            return cartDetails;
        }

        public async Task<bool> Delete(Guid id)
        {
            
            await DeleteAsync(id);
            await SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CartDetail>> GetAllCartDetail()
        {
            var cartDetail = await _context.cartDetails.Include(p => p.ProductDetails.Product).Include(p => p.Cart).Select(p=> new CartDetail
            {
                Id = p.Id,
                CartId = p.CartId,
                ProductDetailId = p.ProductDetailId,
                PromotionDetailId = p.PromotionDetailId,
                Discount = p.Discount,
                Quantity = p.Quantity,
                Price = p.Price,
                Status = p.Status,
            }).ToListAsync();
            return cartDetail;
        }

        public async Task<CartDetail> GetCartDetailById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<CartDetail> Update(CartDetail cartDetail)
        {
            var updateCartDecaill = await _context.cartDetails.FindAsync(cartDetail.Id);
            if (updateCartDecaill == null)
            {
                throw new KeyNotFoundException($"Image with Id {cartDetail.Id} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == cartDetail.ProductDetailId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {cartDetail.ProductDetailId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            updateCartDecaill.ProductDetailId = cartDetail.ProductDetailId;
            await UpdateAsync(updateCartDecaill);
            await SaveChangesAsync();
            return updateCartDecaill;
        }
    }
}
