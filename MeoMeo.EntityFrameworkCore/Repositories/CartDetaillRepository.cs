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
            var itemCreate =  await AddAsync(cartDetails);
            return itemCreate;
        }

        public async Task<bool> Delete(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<CartDetail>> GetAllCartDetail()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }

        public async Task<CartDetail> GetCartDetailById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<CartDetail> Update(CartDetail cartDetail)
        {
            var itemUpdate = await UpdateAsync(cartDetail);
            return itemUpdate;
        }
    }
}
