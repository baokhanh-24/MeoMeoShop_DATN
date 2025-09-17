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
using Microsoft.EntityFrameworkCore;

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

        public async Task<ColourResponseDTO> CreateColourAsync(ColourDTO colourDTO)
        {
            // Check trùng Name
            var isNameExist = await _colourRepository.AnyAsync(x => x.Name == colourDTO.Name);
            if (isNameExist)
            {
                return new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tên màu sắc đã tồn tại."
                };
            }

            // Tự động generate Code
            var colourCode = await GenerateColourCodeAsync();

            var colour = new Colour
            {
                Id = Guid.NewGuid(),
                Name = colourDTO.Name,
                Code = colourCode,
                Status = colourDTO.Status,
            };
            await _colourRepository.Create(colour);
            return new ColourResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm màu sắc thành công"
            };
        }

        public async Task<ColourResponseDTO> DeleteColourAsync(Guid id)
        {
            var image = await _colourRepository.GetColourById(id);
            if (image == null)
            {
                return new ColourResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }

            await _colourRepository.Delete(id);
            return new ColourResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Xóa thành công" };
        }

        public async Task<IEnumerable<Colour>> GetAllColoursAsync()
        {
            var colour = await _colourRepository.GetAllColour();
            return colour;
        }

        public async Task<ColourResponseDTO> GetColourByIdAsync(Guid id)
        {
            var colour = await _colourRepository.GetColourById(id);
            if (colour == null)
            {
                return new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy Colour với ID: {id}"
                };
            }
            return new ColourResponseDTO
            {
                Id = colour.Id,
                Name = colour.Name,
                Code = colour.Code,
                Status = colour.Status,
                ResponseStatus = BaseStatus.Success,
                Message = $""
            };
        }

        public async Task<ColourResponseDTO> UpdateColourAsync(ColourDTO colourDTO)
        {
            var colour = await _colourRepository.GetColourById(colourDTO.Id);
            if (colour == null)
            {
                return new ColourResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy màu sắc cần cập nhật" };
            }

            // Check trùng Name (trừ record hiện tại)
            var isNameExist = await _colourRepository.AnyAsync(x => x.Name == colourDTO.Name && x.Id != colourDTO.Id);
            if (isNameExist)
            {
                return new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tên màu sắc đã tồn tại."
                };
            }

            // Giữ nguyên Code, chỉ cập nhật các trường khác
            var originalCode = colour.Code;
            _mapper.Map(colourDTO, colour);
            colour.Code = originalCode; // Giữ nguyên Code cũ

            await _colourRepository.Update(colour);
            return new ColourResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật màu sắc thành công" };
        }

        private async Task<string> GenerateColourCodeAsync()
        {
            var lastColour = await _colourRepository.Query()
                .OrderByDescending(x => x.Code)
                .FirstOrDefaultAsync();

            if (lastColour == null || string.IsNullOrEmpty(lastColour.Code))
            {
                return "COLOR001";
            }

            // Extract number from last code (e.g., "COLOR001" -> 1)
            var lastCode = lastColour.Code;
            if (lastCode.StartsWith("COLOR"))
            {
                var numberPart = lastCode.Substring(5); // Remove "COLOR"
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    return $"COLOR{(lastNumber + 1):D3}";
                }
            }

            // Fallback: count existing colours
            var count = await _colourRepository.Query().CountAsync();
            return $"COLOR{(count + 1):D3}";
        }
    }
}
