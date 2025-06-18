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

        public async Task<Image> GetImageByIdAsync(Guid id)
        {
            var image = await _imageRepository.GetImageById(id);
            return image;
        }

        public async Task<Image> CreateImageAsync(ImageDTO imageDto)
        {
            var image = new Image
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                URL = imageDto.UrlImg,
                Name = imageDto.Name,
                ProductDetailId = imageDto.ProductDetailId
                // gán thêm các trường khác nếu có
            };
            //var image = _mapper.Map<Image>(imageDto);
            var updated =  await _imageRepository.CreateImage(image);
            return updated;
        }

        public async Task<Image> UpdateImageAsync(ImageDTO imageDto)
        {
            var image = await _imageRepository.GetByIdAsync(imageDto.Id.Value);
            if (image == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            _mapper.Map(imageDto, image);

            await _imageRepository.UpdateAsync(image);
            return image;
            ////Image img = new Image();
            ////var image = _mapper.Map<Image>(imageDto);
            //var updated = await _imageRepository.UpdateImage(image);
            //return updated;
        }

        public async Task<bool> DeleteImageAsync(Guid id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null)
                throw new ArgumentException("Không tìm thấy ảnh với ID đã cung cấp");

            await _imageRepository.DeleteImage(id);
            return true;
            //await _imageRepository.DeleteImage(id);
            //return true;
        }
    }
}
