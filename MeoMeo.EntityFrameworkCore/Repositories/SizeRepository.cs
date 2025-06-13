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
    public class SizeRepository : ISizeRepository
    {
        private readonly MeoMeoDbContext _context;
        public SizeRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task Create(Size size)
        {
            bool exists = await _context.sizes.AnyAsync(p=> p.Id == size.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await _context.sizes.AddAsync(size);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var deleteItem = await _context.sizes.FindAsync(id);
            if (deleteItem == null)
            {
                throw new KeyNotFoundException($"Size with Id {id} not found.");
            }
            _context.sizes.Remove(deleteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Size>> GetAllSize()
        {
            var size = await _context.sizes.Select(p => new Size
            {
                Id = p.Id,
                Value = p.Value,
                Code = p.Code,
                Status = p.Status,
            }).ToListAsync();
            return size;
        }

        public async Task<Size> GetSizeById(Guid id)
        {
            return await _context.sizes.FindAsync(id);
        }

        public async Task Update(Guid Id)
        {
            var updateSize = await _context.sizes.FindAsync(Id);
            if (updateSize == null)
            {
                throw new KeyNotFoundException($"Image with Id {Id} not found.");
            }
            // Cập nhật nếu hợp lệ
            _context.sizes.Update(updateSize);
            await _context.SaveChangesAsync();
        }
    }
}
