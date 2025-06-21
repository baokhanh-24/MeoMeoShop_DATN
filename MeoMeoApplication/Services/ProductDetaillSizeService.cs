using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        public async Task<ProductDetaillSizeResponseDTO> CreateProductDetaillSizeAsync(ProductDetaillSizeDTO productDetaillSizeDTO)
        {
            var productDetailSize = new ProductDetailSize
            {
                SizeId = productDetaillSizeDTO.SizeId,
                ProductDetailId = productDetaillSizeDTO.ProductDetaillId
            };
            var updated = await _productDetaillSizeRepository.Create(productDetailSize);
            return new ProductDetaillSizeResponseDTO
            {
                Status = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public async Task<bool> DeleteProductDetaillSizeAsync(Guid id)
        {
            await _productDetaillSizeRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ProductDetailSize>> GetAllProductDetaillSizeAsync()
        {
            var productDetailSizes = await _productDetaillSizeRepository.GetAllProductDetaillSize();
            return productDetailSizes;
        }

        public async Task<ProductDetailSize> GetProductDetaillSizeByIdAsync(Guid id)
        {
            var productDetailSize = await _productDetaillSizeRepository.GetProductDetaillSizeById(id);
            return productDetailSize;
        }

        public async Task<ProductDetaillSizeResponseDTO> UpdateProductDetaillSizeAsync(ProductDetaillSizeDTO productDetaillSizeDTO)
        {
            var productDetailSize = await _productDetaillSizeRepository.GetProductDetaillSizeById(productDetaillSizeDTO.SizeId);
            if (productDetailSize == null)
            {
                return new ProductDetaillSizeResponseDTO { Status = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(productDetaillSizeDTO, productDetailSize);

            await _productDetaillSizeRepository.Update(productDetailSize);
            return new ProductDetaillSizeResponseDTO { Status = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
