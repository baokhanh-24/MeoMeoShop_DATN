using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
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
        private readonly IProductSeasonRepository _productSeasonRepository;
        public ProductSeasonService(IProductSeasonRepository productSeasonRepository)
        {
            _productSeasonRepository = productSeasonRepository;
        }

        public async Task<ProductSeason> CreateProductSeasonAsync(ProductSeasonDTO dto)
        {
            var productSeason = new ProductSeason
            {
                ProductId = dto.ProductId,
                SeasonId = dto.SeasonId,
            };

            await _productSeasonRepository.CreateProductSeasonAsync(productSeason);

            try
            {
                await _productSeasonRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Lấy lỗi chi tiết từ inner exception
                throw new Exception($"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}");
            }

            return productSeason;
        }

        public async Task<bool> DeleteProductSeasonAsync(Guid ProductId, Guid SeasonId)
        {
            var phat = await _productSeasonRepository.GetProductSeasonByIdAsync(ProductId, SeasonId);
            if (phat == null)
            {
                return false;
            }
            _productSeasonRepository.RemoveProductSeason(phat);
            await _productSeasonRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductSeasonDTO>> GetAllProductSeasonAsync()
        {
            return await _productSeasonRepository.GeProductSeasontAllAsync()
                .ContinueWith(task => task.Result.Select(ps => new ProductSeasonDTO
                {
                    ProductId = ps.ProductId,
                    SeasonId = ps.SeasonId,
                }));
        }

        public async Task<ProductSeason> GetProductSeasonByIdAsync(Guid ProductId, Guid SeasonId)
        {
            return await _productSeasonRepository.GetProductSeasonByIdAsync(ProductId, SeasonId);
        }

        public async Task<ProductSeason> UpdateProductSeasonAsync(Guid ProductId, Guid Seasonid, ProductSeasonDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var old = await _productSeasonRepository.GetProductSeasonByIdAsync(ProductId, Seasonid);
            if (old == null)
                throw new Exception("Không tìm thấy bản ghi cần cập nhật");

            // Nếu không đổi khóa, thì không cần xóa –> return luôn
            if (ProductId == dto.ProductId && Seasonid == dto.SeasonId)
            {
                return old;
            }

            // Xóa bản ghi cũ
            await _productSeasonRepository.DeleteProductSeasonAsync(ProductId, Seasonid);

            // Kiểm tra tránh insert trùng
            var existed = await _productSeasonRepository.GetProductSeasonByIdAsync(dto.ProductId, dto.SeasonId);
            if (existed != null)
            {
                throw new Exception("Bản ghi với khóa mới đã tồn tại.");
            }

            // Tạo bản ghi mới
            var newProductSeason = new ProductSeason
            {
                ProductId = dto.ProductId,
                SeasonId = dto.SeasonId,
            };

            await _productSeasonRepository.CreateProductSeasonAsync(newProductSeason);
            await _productSeasonRepository.SaveChangesAsync();

            return newProductSeason;
        }
    }
}
