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
    public class ProductDetaillSizeRepository : IProductDetaillSizeRepository
    {
        private readonly MeoMeoDbContext _context;
        public ProductDetaillSizeRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task Create(ProductDetailSize productDetailSize)
        {
            bool exists = await _context.productDetailSizes.AnyAsync(c => c.SizeId == productDetailSize.SizeId);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This ProductDetaillColor is existed!");
            }
            await _context.productDetailSizes.AddAsync(productDetailSize);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var deleteItem = await _context.productDetailSizes.FindAsync(id);
            if (deleteItem == null)
            {
                throw new KeyNotFoundException($"ProductDetaillSize with Id {id} not found.");
            }
            _context.productDetailSizes.Remove(deleteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductDetailSize>> GetAllProductDetaillSize()
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
            return await _context.productDetailSizes.FindAsync(id);
        }

        public async Task Update(Guid Id, Guid ProductDetaillId)
        {
            var update = await _context.productDetailSizes.FindAsync(Id);
            if (update == null)
            {
                throw new KeyNotFoundException($"ProductDetaillSize with Id {Id} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == ProductDetaillId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {ProductDetaillId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            update.ProductDetailId = ProductDetaillId;
            _context.productDetailSizes.Update(update);
            await _context.SaveChangesAsync();
        }
    }
}
