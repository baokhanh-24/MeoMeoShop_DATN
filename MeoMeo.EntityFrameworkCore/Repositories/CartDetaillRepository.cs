using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class CartDetaillRepository : ICartDetaillRepository
    {
        private readonly MeoMeoDbContext _context;
        public CartDetaillRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task Create(CartDetail cartDetails)
        {
            bool exists = await _context.cartDetails.AnyAsync(c => c.Id == cartDetails.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await _context.cartDetails.AddAsync(cartDetails);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var deleteItem = await _context.cartDetails.FindAsync(id);
            if (deleteItem == null)
            {
                throw new KeyNotFoundException($"CartDetail with Id {id} not found.");
            }
            _context.cartDetails.Remove(deleteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CartDetail>> GetAllCartDetail()
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
            return await _context.cartDetails.FindAsync(id);
        }

        public async Task Update(Guid cartDetailId, Guid productId, int quantity)
        {
            var updateCartDecaill = await _context.cartDetails.FindAsync(cartDetailId);
            if (updateCartDecaill == null)
            {
                throw new KeyNotFoundException($"Image with Id {cartDetailId} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == productId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {productId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            updateCartDecaill.ProductDetailId = productId;
            _context.cartDetails.Update(updateCartDecaill);
            await _context.SaveChangesAsync();
        }
    }
}
