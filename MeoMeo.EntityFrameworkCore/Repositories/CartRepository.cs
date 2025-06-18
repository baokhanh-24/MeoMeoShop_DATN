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
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        public CartRepository(MeoMeoDbContext context) :base(context)
        {
           
        }

        public async Task<Cart> Create(Cart cart)
        {
            bool exists = await _context.carts.AnyAsync(c => c.Id == cart.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await AddAsync(cart);
            await SaveChangesAsync();
            return cart;
        }

        public async Task<IEnumerable<Cart>> GetAllCart()
        {
            var Img = await _context.carts.Include(p => p.Customers).Select(p => new Cart
            {
                Id = p.Id,
                CustomerId = p.CustomerId,
                CreatedBy = p.CreatedBy,
                LastModificationTime = p.LastModificationTime,
                TotalPrice = p.TotalPrice,
                UpdatedBy = p.UpdatedBy,
            }).ToListAsync();
            return Img;
        }

        public async Task<Cart> GetCartById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task Savechanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> Update(Cart cart)
        {
            var updateImg = await _context.carts.FindAsync(cart.Id);
            if (updateImg == null)
            {
                throw new KeyNotFoundException($"Image with Id {cart.Id} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.customers.AnyAsync(p => p.Id == cart.CustomerId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {cart.CustomerId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            updateImg.CustomerId= cart.CustomerId;
            await UpdateAsync(updateImg);
            await SaveChangesAsync();
            return updateImg;
        }
    }
}
