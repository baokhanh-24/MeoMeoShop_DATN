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
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IMapper _mapper;
        public SizeService(ISizeRepository sizeRepository, IMapper mapper)
        {
            _sizeRepository = sizeRepository;
            _mapper = mapper;
        }

        public async Task<Size> CreateSizeAsync(SizeDTO sizeDTO)
        {
            var image = new Size
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                Value = sizeDTO.Value,
                Code = sizeDTO.Code
               
                // gán thêm các trường khác nếu có
            };
            var updated = await _sizeRepository.Create(image);
            return updated;
        }

        public async Task<bool> DeleteSizeAsync(Guid id)
        {
            var image = await _sizeRepository.GetSizeById(id);
            if (image == null)
                throw new ArgumentException("Không tìm thấy ảnh với ID đã cung cấp");

            await _sizeRepository.Delete(id);
            return true;
        }

        public async Task<IEnumerable<Size>> GetAllSizeAsync()
        {
            var images = await _sizeRepository.GetAllSize();
            return images;
        }

        public async Task<Size> GetSizeByIdAsync(Guid id)
        {
            var image = await _sizeRepository.GetSizeById(id);
            return image;
        }

        public async Task<Size> UpdateSizeAsync(SizeDTO sizeDTO)
        {
            var image = await _sizeRepository.GetSizeById(sizeDTO.Id.Value);
            if (image == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            _mapper.Map(sizeDTO, image);

            await _sizeRepository.Update(image);
            return image;
        }
    }
}
