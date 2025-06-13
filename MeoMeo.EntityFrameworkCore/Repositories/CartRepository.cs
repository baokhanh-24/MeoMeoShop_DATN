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

        public async Task Create(Cart cart)
        {
            if (await GetCartById(cart.Id) != null) throw new DuplicateWaitObjectException($"Cart : {cart.Id} is existed");
            await  _context.carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Cart>> GetAllCart()
        {
            return await _context.carts.Include(p => p.Customers).Select(p=> new Cart
            {
                Id = p.Id,
                CustomerId = p.CustomerId,
                TotalPrice = p.TotalPrice,
                LastModificationTime = p.LastModificationTime,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy,
                CreationTime = p.CreationTime,
            }).ToListAsync();
        }

        public async Task<Cart> GetCartById(Guid id)
        {
            return await _context.carts.Include(p => p.Customers).Select(p=>  new Cart
            {
                Id = p.Id,
                CustomerId = p.CustomerId,
                TotalPrice = p.TotalPrice,
                LastModificationTime = p.LastModificationTime,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy,
                CreationTime = p.CreationTime,
            }).FirstOrDefaultAsync();
        }

        public async Task Savechanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Cart cart)
        {
            if (await GetCartById(cart.Id) == null) throw new KeyNotFoundException("Not found this Id");
            _context.Entry(cart).State = EntityState.Modified;
        }
    }
}
