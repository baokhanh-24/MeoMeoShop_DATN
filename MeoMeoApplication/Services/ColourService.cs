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
            var colour = new Colour
            {
                Id = Guid.NewGuid(), 
                Name = colourDTO.Name,
                Code = colourDTO.Code,
                Status = colourDTO.Status,
            };
            await _colourRepository.Create(colour);
            return new ColourResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm thành công"
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
            if(colour == null)
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
            var colour = await _colourRepository.GetColourById(colourDTO.Id.Value);
            if (colour == null)
            {
                return new ColourResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(colourDTO, colour);

            await _colourRepository.Update(colour);
            return new ColourResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
