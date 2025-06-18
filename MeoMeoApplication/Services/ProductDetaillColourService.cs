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
    public class ProductDetaillColourService : IProductDetaillColourService
    {
        private readonly IProductDetaillColourRepository _productDetaillColourRepository;
        private readonly IMapper _mapper;
        public ProductDetaillColourService(IProductDetaillColourRepository productDetaillColourRepository, IMapper mapper)
        {
            _productDetaillColourRepository = productDetaillColourRepository;
            _mapper = mapper;
        }

        public async Task<ProductDetailColour> CreateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO)
        {
            var image = new ProductDetailColour
            {
                ProductDetailId =  productDetaillColourDTO.ProductDetaillId,
                ColourId = productDetaillColourDTO.ColourId,
                
                // gán thêm các trường khác nếu có
            };
            var updated = await _productDetaillColourRepository.Create(image);
            return updated;
        }

        public Task<bool> DeleteProduuctDetaillColourAsync(Guid id)
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

        public async Task<ProductDetailColour> UpdateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO)
        {
            var image = await _productDetaillColourRepository.GetProductDetaillColourById(productDetaillColourDTO.ColourId);
            if (image == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            _mapper.Map(productDetaillColourDTO, image);

            await _productDetaillColourRepository.Update(image);
            return image;
        }
    }
}
