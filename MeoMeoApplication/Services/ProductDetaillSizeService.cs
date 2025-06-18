using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class ProductDetaillSizeService : IProductDetaillSizeService
    {
        private readonly IProductDetaillSizeRepository _productDetaillSizeRepository;
        private readonly IMapper _mapper;
        public ProductDetaillSizeService(IProductDetaillSizeRepository productDetaillSizeRepository, IMapper mapper)
        {
            _productDetaillSizeRepository = productDetaillSizeRepository;
            _mapper = mapper;
        }

        public async Task<ProductDetailSize> CreateProductDetaillSizeAsync(ProductDetaillSizeDTO productDetaillSizeDTO)
        {
            var image = new ProductDetailSize
            {
                SizeId = productDetaillSizeDTO.SizeId,
                ProductDetailId = productDetaillSizeDTO.ProductDetaillId
                // gán thêm các trường khác nếu có
            };
            var updated = await _productDetaillSizeRepository.Create(image);
            return updated;
        }

        public async Task<bool> DeleteProductDetaillSizeAsync(Guid id)
        {
            await _productDetaillSizeRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ProductDetailSize>> GetAllProductDetaillSizeAsync()
        {
            var images = await _productDetaillSizeRepository.GetAllProductDetaillSize();
            return images;
        }

        public async Task<ProductDetailSize> GetProductDetaillSizeByIdAsync(Guid id)
        {
            var image = await _productDetaillSizeRepository.GetProductDetaillSizeById(id);
            return image;
        }

        public async Task<ProductDetailSize> UpdateProductDetaillSizeAsync(ProductDetaillSizeDTO productDetaillSizeDTO)
        {
            var image = await _productDetaillSizeRepository.GetProductDetaillSizeById(productDetaillSizeDTO.SizeId);
            if (image == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            _mapper.Map(productDetaillSizeDTO, image);

            await _productDetaillSizeRepository.Update(image);
            return image;
        }
    }
}
