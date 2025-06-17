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
        private readonly MeoMeoDbContext _context;
        public SizeRepository(MeoMeoDbContext context) : base(context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Size> Create(Size size)
        {
            bool exists = await _context.sizes.AnyAsync(p=> p.Id == size.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await AddAsync(size);
            await SaveChangesAsync();
            return size;
        }

        public async Task<bool> Delete(Guid id)
        {          
            await DeleteAsync(id);
            await SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Size>> GetAllSize()
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
            return await GetByIdAsync(id);
        }

        public async Task<Size> Update(Size size)
        {
            var updateSize = await _context.sizes.FindAsync(size.Id);
            if (updateSize == null)
            {
                throw new KeyNotFoundException($"Size with Id {size.Id} not found.");
            }
            // Cập nhật nếu hợp lệ
            await UpdateAsync(updateSize);
            await SaveChangesAsync();
            return updateSize;
        }
    }
}
