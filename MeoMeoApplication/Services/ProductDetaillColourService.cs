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
    public class ProductDetaillColourService : IProductDetaillColourService
    {
        private readonly IProductDetaillColourRepository _productDetaillColourRepository;
        private readonly IMapper _mapper;
        public ProductDetaillColourService(IProductDetaillColourRepository productDetaillColourRepository, IMapper mapper)
        {
            _productDetaillColourRepository = productDetaillColourRepository;
            _mapper = mapper;
        }

        public async Task<ProductDetaillColourResponseDTO> CreateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO)
        {
            var productDetailColour = new ProductDetailColour
            {
                ProductDetailId =  productDetaillColourDTO.ProductDetaillId,
                ColourId = productDetaillColourDTO.ColourId,
                
            };
            await _productDetaillColourRepository.Create(productDetailColour);
            return new ProductDetaillColourResponseDTO
            {
                Status = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public Task<ProductDetaillColourResponseDTO> DeleteProduuctDetaillColourAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductDetailColour>> GetAllProductDetaillColourAsync()
        {
            var images = await _productDetaillColourRepository.GetAllProductDetaillColour();
            return images;
        }

        public async Task<ProductDetailColour> GetProductDetaillColourByIdAsync(Guid id)
        {
            var image = await _productDetaillColourRepository.GetProductDetaillColourById(id);
            return image;
        }

        public async Task<ProductDetaillColourResponseDTO> UpdateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO)
        {
            var image = await _productDetaillColourRepository.GetProductDetaillColourById(productDetaillColourDTO.ColourId);
            if (image == null)
            {
                return new ProductDetaillColourResponseDTO { Status = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }

            _mapper.Map(productDetaillColourDTO, image);

            await _productDetaillColourRepository.Update(image);
            return new ProductDetaillColourResponseDTO { Status = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
