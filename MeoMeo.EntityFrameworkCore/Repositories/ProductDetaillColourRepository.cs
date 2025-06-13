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
    public class ProductDetaillColourRepository : IProductDetaillColourRepository
    {
        private readonly MeoMeoDbContext _context;
        public ProductDetaillColourRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task Create(ProductDetailColour productDetailColour)
        {
            bool exists = await _context.productDetailColours.AnyAsync(c => c.ColourId == productDetailColour.ColourId);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This ProductDetaillColor is existed!");
            }
            await _context.productDetailColours.AddAsync(productDetailColour);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var deleteItem = await _context.productDetailColours.FindAsync(id);
            if (deleteItem == null)
            {
                throw new KeyNotFoundException($"ProductDetaillColour with Id {id} not found.");
            }
            _context.productDetailColours.Remove(deleteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductDetailColour>> GetAllProductDetaillColour()
        {
            var Img = await _context.productDetailColours.Include(p => p.ProductDetail).Select(p => new ProductDetailColour
            {
                ColourId = p.ColourId,
                ProductDetailId = p.ProductDetailId,

            }).ToListAsync();
            return Img;
        }

        public async Task<ProductDetailColour> GetProductDetaillColourById(Guid id)
        {
            return await _context.productDetailColours.FindAsync(id);
        }

        public async Task Update(Guid Id, Guid ProductDetaillId)
        {
            var update = await _context.productDetailColours.FindAsync(Id);
            if (update == null)
            {
                throw new KeyNotFoundException($"Image with Id {Id} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == ProductDetaillId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {ProductDetaillId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            update.ProductDetailId = ProductDetaillId;
            _context.productDetailColours.Update(update);
            await _context.SaveChangesAsync();
        }
    }
}
