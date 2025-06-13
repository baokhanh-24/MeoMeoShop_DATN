using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class ProductSeasonService : IProductSeasonServices
    {
        private readonly MeoMeoDbContext _context;
        public ProductSeasonService(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task<ProductSeasonDTO> CreateAsync(ProductSeasonDTO dto)
        {
            // Kiểm tra xem đã tồn tại chưa
            bool exists = await _context.productSeasons
                .AnyAsync(ps => ps.SeasonId == dto.SeasonId && ps.ProductId == dto.ProductId);

            if (exists)
            {
                // Nếu đã tồn tại thì có thể trả về lỗi hoặc thông báo
                throw new Exception("Bản ghi ProductSeason đã tồn tại.");
            }

            var newProductSeason = new ProductSeason
            {
                SeasonId = dto.SeasonId,
                ProductId = dto.ProductId
            };

            _context.productSeasons.Add(newProductSeason);
            await _context.SaveChangesAsync();

            return new ProductSeasonDTO
            {
                SeasonId = newProductSeason.SeasonId,
                ProductId = newProductSeason.ProductId
            };
        }


        public async Task<bool> DeleteAsync(Guid ProductId, Guid Seasonid)
        {
            var entity = await _context.productSeasons
            .FirstOrDefaultAsync(x => x.ProductId == ProductId && x.SeasonId == Seasonid);

            if (entity == null) return false;

            _context.productSeasons.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductSeasonDTO>> GetAllAsync()
        {
            return await _context.productSeasons.Select(x => new ProductSeasonDTO
            {
                SeasonId = x.SeasonId,
                ProductId = x.ProductId
            }).ToListAsync();
        }

        public async Task<ProductSeasonDTO> GetByIdAsync(Guid ProductId, Guid SeasonId)
        {
            var entity = await _context.productSeasons
        .Include(x => x.Product)
        .Include(x => x.Season)
        .FirstOrDefaultAsync(x => x.ProductId == ProductId && x.SeasonId == SeasonId);

            if (entity == null) return null;

            return new ProductSeasonDTO
            {
                ProductId = entity.ProductId,
                SeasonId = entity.SeasonId
            };
        }

        public async Task<ProductSeasonDTO> UpdateAsync(Guid Productid, Guid SeasonId, ProductSeasonDTO dto)
        {
            var existing = await _context.productSeasons
        .FirstOrDefaultAsync(x => x.ProductId == Productid && x.SeasonId == SeasonId);

            if (existing == null) return null;

            // Vì ProductId + SeasonId là composite key nên không nên update chúng trực tiếp
            // Cách an toàn là: xóa bản ghi cũ và thêm bản ghi mới
            _context.productSeasons.Remove(existing);

            var newEntry = new ProductSeason
            {
                ProductId = dto.ProductId,
                SeasonId = dto.SeasonId
            };

            _context.productSeasons.Add(newEntry);
            await _context.SaveChangesAsync();

            return new ProductSeasonDTO
            {
                ProductId = newEntry.ProductId,
                SeasonId = newEntry.SeasonId
            };
        }
    }
}
