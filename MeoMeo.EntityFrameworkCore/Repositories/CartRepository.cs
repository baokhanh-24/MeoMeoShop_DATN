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
            var itemCreate = await AddAsync(cart);
            return itemCreate;
        }

        public async Task<IEnumerable<Cart>> GetAllCart()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
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
            var itemUpdate = await UpdateAsync(cart);
            return itemUpdate;
        }
        
    }
}
