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
    public class ImageRepository : IImageRepository
    {
        private readonly MeoMeoDbContext _context;
        public ImageRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task Create(Image img)
        {
            bool exists = await _context.images.AnyAsync(c => c.Id == img.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await _context.images.AddAsync(img);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var deleteItem = await _context.images.FindAsync(id);
            if (deleteItem == null)
            {
                throw new KeyNotFoundException($"Image with Id {id} not found.");
            }
            _context.images.Remove(deleteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Image>> GetAllImage()
        {
            var Img = await _context.images.Include(p => p.ProductDetail).Select(p => new Image
            {
                Id = p.Id,
                ProductDetailId = p.ProductDetailId,
                Name = p.Name,
                Type = p.Type,
                URL = p.URL,

            }).ToListAsync();
            return Img;
        }

        public async Task<Image> GetImageById(Guid id)
        {
            return await _context.images.FindAsync(id);
        }

        public async Task Update(Guid Id, Guid producDetailId)
        {
            var updateImg = await _context.images.FindAsync(Id);
            if(updateImg == null)
            {
                throw new KeyNotFoundException($"Image with Id {Id} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == producDetailId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {producDetailId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            updateImg.ProductDetailId = producDetailId;
            _context.images.Update(updateImg);
            await _context.SaveChangesAsync();
        }
    }
}
