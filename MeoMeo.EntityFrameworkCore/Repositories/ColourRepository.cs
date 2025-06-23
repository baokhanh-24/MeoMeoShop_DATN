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
    public class ColourRepository : BaseRepository<Colour>, IColourRepository
    {      
        public ColourRepository(MeoMeoDbContext context) : base(context)
        {
            
        }
        public async Task<Colour> Create(Colour colour)
        {
            var itemCreate = await AddAsync(colour);
            return itemCreate;
        }

        public async Task<bool> Delete(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Colour>> GetAllColour()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }

        public async Task<Colour> GetColourById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Colour> Update(Colour colour)
        {
            var itemUpdate = await UpdateAsync(colour);
            return itemUpdate;
        }
    }
}
