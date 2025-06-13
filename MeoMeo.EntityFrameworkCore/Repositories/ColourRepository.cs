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
    public class ColourRepository : IColourRepository
    {
        private readonly MeoMeoDbContext _context;
        public ColourRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task Create(Colour colour)
        {
            bool exists = await _context.colours.AnyAsync(c => c.Id == colour.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await _context.colours.AddAsync(colour);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var deleteItem = await _context.colours.FindAsync(id);
            if (deleteItem == null)
            {
                throw new KeyNotFoundException($"Colours with Id {id} not found.");
            }
            _context.colours.Remove(deleteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Colour>> GetAllColour()
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
            return await _context.colours.FindAsync(id);
        }

        public async Task Update(Guid Id)
        {
            var updateColour = await _context.colours.FindAsync(Id);
            if (updateColour == null)
            {
                throw new KeyNotFoundException($"Image with Id {Id} not found.");
            }
            // Cập nhật nếu hợp lệ
            _context.colours.Update(updateColour);
            await _context.SaveChangesAsync();
        }
    }
}
