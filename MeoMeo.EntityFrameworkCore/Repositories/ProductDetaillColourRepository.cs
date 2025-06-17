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
    public class ProductDetaillColourRepository : BaseRepository<ProductDetailColour>, IProductDetaillColourRepository
    {
        public ProductDetaillColourRepository(MeoMeoDbContext context) : base(context) 
        {
            
        }
        public async Task<ProductDetailColour> Create(ProductDetailColour productDetailColour)
        {
            bool exists = await _context.productDetailColours.AnyAsync(c => c.ColourId == productDetailColour.ColourId);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This ProductDetaillColor is existed!");
            }
            await AddAsync(productDetailColour);
            await SaveChangesAsync();
            return productDetailColour;
        }

        public async Task<bool> Delete(Guid id)
        {           
            await DeleteAsync (id);
            await SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDetailColour>> GetAllProductDetaillColour()
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
            return await GetByIdAsync(id);
        }

        public async Task<ProductDetailColour> Update(ProductDetailColour productDetailColour)
        {
            var update = await _context.productDetailColours.FindAsync(productDetailColour.ProductDetailId);
            if (update == null)
            {
                throw new KeyNotFoundException($"Image with Id {productDetailColour.ProductDetailId} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == productDetailColour.ProductDetailId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {productDetailColour.ProductDetailId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            update.ProductDetailId = productDetailColour.ProductDetailId;
            await UpdateAsync(update);
            await SaveChangesAsync();
            return update;
        }
    }
}
