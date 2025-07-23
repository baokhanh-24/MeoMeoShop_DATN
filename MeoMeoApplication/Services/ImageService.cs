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
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageService(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Image>> GetAllImagesAsync()
        {
            var images = await _imageRepository.GetAllImage();
            return images;
        }

        public async Task<ImageResponseDTO> GetImageByIdAsync(Guid id)
        {
            var image = await _imageRepository.GetImageById(id);
            if(image == null)
            {
                return new ImageResponseDTO 
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy Image với ID: {id}"
                };
            }
            return new ImageResponseDTO 
            {
                Id = image.Id,
                ProductDetailId = image.ProductDetailId,
                Name = image.Name,
                Url = image.URL,
                Type = image.Type,
                ResponseStatus = BaseStatus.Success,
                Message = $""
            };
        }

        public async Task<ImageResponseDTO> CreateImageAsync(ImageDTO imageDto)
        {
            var image = new Image
            {
                Id = Guid.NewGuid(), 
                URL = imageDto.Url,
                Name = imageDto.Name,
                ProductDetailId = imageDto.ProductDetailId,
            };
            //var image = _mapper.Map<Image>(imageDto);
            var updated =  await _imageRepository.CreateImage(image);
            return new ImageResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public async Task<ImageResponseDTO> UpdateImageAsync(ImageDTO imageDto)
        {
            var image = await _imageRepository.GetImageById(imageDto.Id.Value);
            if (image == null)
            {
                return new ImageResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }

            _mapper.Map(imageDto, image);
            await _imageRepository.UpdateImage(image);
            return new ImageResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }

        public async Task<ImageResponseDTO> DeleteImageAsync(Guid id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null)
            {
                return new ImageResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }
            await _imageRepository.DeleteImage(id);
            return new ImageResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Xóa thành công" };
        }
    }
}
