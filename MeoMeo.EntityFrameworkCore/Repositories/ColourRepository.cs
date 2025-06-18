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
            bool exists = await _context.colours.AnyAsync(c => c.Id == colour.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await AddAsync(colour);
            await SaveChangesAsync();
            return colour;
        }

        public async Task<bool> Delete(Guid id)
        {
            await DeleteAsync(id);
            await SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Colour>> GetAllColour()
        {
            var colour = await _context.colours.Select(p => new Colour
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                Status = p.Status,
            }).ToListAsync();
            return colour;
        }

        public async Task<Colour> GetColourById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Colour> Update(Colour colour)
        {
            var updateImg = await _context.colours.FindAsync(colour.Id);
            if (updateImg == null)
            {
                throw new KeyNotFoundException($"Image with Id {colour.Id} not found.");
            }
            await UpdateAsync(updateImg);
            await SaveChangesAsync();
            return updateImg;
        }
    }
}
