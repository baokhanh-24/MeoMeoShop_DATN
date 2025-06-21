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
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public Task<ProductDetaillColourResponseDTO> DeleteProduuctDetaillColourAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductDetailColour>> GetAllProductDetaillColourAsync()
        {
            var productDetailColours = await _productDetaillColourRepository.GetAllProductDetaillColour();
            return productDetailColours;
        }

        public async Task<ProductDetaillColourResponseDTO> GetProductDetaillColourByIdAsync(Guid id)
        {
            var productDetailColour = await _productDetaillColourRepository.GetProductDetaillColourById(id);
            if(productDetailColour == null)
            {
                return new ProductDetaillColourResponseDTO 
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy ID: {id}"
                };
            }
            return new ProductDetaillColourResponseDTO 
            {
                ColourId = productDetailColour.ColourId,
                ProductDetaillId = productDetailColour.ProductDetailId,
                ResponseStatus = BaseStatus.Success,
                Message = $""
            };
        }

        public async Task<ProductDetaillColourResponseDTO> UpdateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO)
        {
            var productDetailColour = await _productDetaillColourRepository.GetProductDetaillColourById(productDetaillColourDTO.ColourId);
            if (productDetailColour == null)
            {
                return new ProductDetaillColourResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }

            _mapper.Map(productDetaillColourDTO, productDetailColour);

            await _productDetaillColourRepository.Update(productDetailColour);
            return new ProductDetaillColourResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
