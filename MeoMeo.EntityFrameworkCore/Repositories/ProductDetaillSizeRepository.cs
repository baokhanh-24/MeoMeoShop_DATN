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
    public class ProductDetaillSizeRepository : BaseRepository<ProductDetailSize>, IProductDetaillSizeRepository
    {
        public ProductDetaillSizeRepository(MeoMeoDbContext context) : base(context)
        {
            
        }
        public async Task<ProductDetailSize> Create(ProductDetailSize productDetailSize)
        {
            bool exists = await _context.productDetailSizes.AnyAsync(c => c.SizeId == productDetailSize.SizeId);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This ProductDetaillColor is existed!");
            }
            await AddAsync(productDetailSize);
            await SaveChangesAsync();
            return productDetailSize;
        }

        public async Task<bool> Delete(Guid id)
        {       
            await DeleteAsync(id);
            await SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDetailSize>> GetAllProductDetaillSize()
        {
            var Pds = await _context.productDetailSizes.Include(p => p.ProductDetail).Select(p => new ProductDetailSize
            {
                SizeId = p.SizeId,
                ProductDetailId = p.ProductDetailId,

            }).ToListAsync();
            return Pds;
        }

        public async Task<ProductDetailSize> GetProductDetaillSizeById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<ProductDetailSize> Update(ProductDetailSize productDetailSize)
        {
            var update = await _context.productDetailSizes.FindAsync(productDetailSize.ProductDetailId);
            if (update == null)
            {
                throw new KeyNotFoundException($"ProductDetaillSize with Id {productDetailSize.ProductDetailId} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == productDetailSize.ProductDetailId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {productDetailSize.ProductDetailId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            update.ProductDetailId = productDetailSize.ProductDetailId;
            await UpdateAsync(update);
            await SaveChangesAsync();
            return update;
        }
    }
}
