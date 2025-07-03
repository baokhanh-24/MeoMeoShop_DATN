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
    public class SizeRepository : BaseRepository<Size>, ISizeRepository
    {

        public SizeRepository(MeoMeoDbContext context) : base(context) 
        {
            
        }
        public async Task<Size> Create(Size size)
        {
            var itemCreate = await AddAsync(size);
            return itemCreate;
        }

        public async Task<bool> Delete(Guid id)
        {          
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Size>> GetAllSize()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }

        public async Task<Size> GetSizeById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Size> Update(Size size)
        {
            var itemUpdate = await UpdateAsync(size);
            return itemUpdate;
        }

        async Task<List<Size>> ISizeRepository.GetAllSize()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }
    }
}
