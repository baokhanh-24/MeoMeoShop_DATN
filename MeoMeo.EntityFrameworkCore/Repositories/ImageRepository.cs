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
    public class ImageRepository : BaseRepository<Image>, IImageRepository
    {
        public ImageRepository(MeoMeoDbContext context) : base(context)
        {
            
        }

        public async Task<Image> CreateImage(Image img)
        {
            bool exists = await _context.images.AnyAsync(c => c.Id == img.Id);
            if (exists)
            {
                throw new DuplicateWaitObjectException("This cartDetails is existed!");
            }
            await AddAsync(img);
            await SaveChangesAsync();
            return img;
        }

        public async Task<bool> DeleteImage(Guid id)
        {
            
            await DeleteAsync(id);
            await SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Image>> GetAllImage()
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
            return await GetByIdAsync(id);
        }

        public async Task<Image> UpdateImage(Image img)
        {
            var updateImg = await _context.images.FindAsync(img.Id);
            if (updateImg == null)
            {
                throw new KeyNotFoundException($"Image with Id {img.Id} not found.");
            }
            // Kiểm tra ProductDetailId có tồn tại hay không
            bool productDetailExists = await _context.productDetails.AnyAsync(p => p.Id == img.ProductDetailId);
            if (!productDetailExists)
            {
                throw new KeyNotFoundException($"ProductDetail with Id {img.ProductDetailId} does not exist.");
            }
            // Cập nhật nếu hợp lệ
            updateImg.ProductDetailId = img.ProductDetailId;
            await UpdateAsync(updateImg);
            await SaveChangesAsync();
            return updateImg;
        }
    }
}
