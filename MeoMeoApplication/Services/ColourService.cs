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
    public class ColourService : IColourService
    {
        private readonly IColourRepository _colourRepository;
        private readonly IMapper _mapper;
        public ColourService(IColourRepository colourRepository, IMapper mapper)
        {
            _colourRepository = colourRepository;
            _mapper = mapper;
        }

        public async Task<Colour> CreateColourAsync(ColourDTO colourDTO)
        {
            var image = new Colour
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                Name = colourDTO.Name,
                Code = colourDTO.Code,
                Status = colourDTO.Status,
                // gán thêm các trường khác nếu có
            };
            var updated = await _colourRepository.Create(image);
            return updated;
        }

        public async Task<bool> DeleteColourAsync(Guid id)
        {
            var image = await _colourRepository.GetColourById(id);
            if (image == null)
                throw new ArgumentException("Không tìm thấy ảnh với ID đã cung cấp");

            await _colourRepository.Delete(id);
            return true;
        }

        public async Task<IEnumerable<Colour>> GetAllColoursAsync()
        {
            var images = await _colourRepository.GetAllColour();
            return images;
        }

        public async Task<Colour> GetColourByIdAsync(Guid id)
        {
            var image = await _colourRepository.GetColourById(id);
            return image;
        }

        public async Task<Colour> UpdateColourAsync(ColourDTO colourDTO)
        {
            var image = await _colourRepository.GetColourById(colourDTO.Id.Value);
            if (image == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            _mapper.Map(colourDTO, image);

            await _colourRepository.Update(image);
            return image;
        }
    }
}
