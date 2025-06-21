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
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IMapper _mapper;
        public SizeService(ISizeRepository sizeRepository, IMapper mapper)
        {
            _sizeRepository = sizeRepository;
            _mapper = mapper;
        }

        public async Task<SizeResponseDTO> CreateSizeAsync(SizeDTO sizeDTO)
        {
            var size = new Size
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                Value = sizeDTO.Value,
                Code = sizeDTO.Code
               
            };
            var updated = await _sizeRepository.Create(size);
            return new SizeResponseDTO
            {
                Status = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public async Task<SizeResponseDTO> DeleteSizeAsync(Guid id)
        {
            var size = await _sizeRepository.GetSizeById(id);
            if (size == null)
            {
                return new SizeResponseDTO { Status = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }

            await _sizeRepository.Delete(id);
            return new SizeResponseDTO { Status = BaseStatus.Success, Message = "Xóa thành công" };
        }

        public async Task<IEnumerable<Size>> GetAllSizeAsync()
        {
            var size = await _sizeRepository.GetAllSize();
            return size;
        }

        public async Task<Size> GetSizeByIdAsync(Guid id)
        {
            var size = await _sizeRepository.GetSizeById(id);
            return size;
        }

        public async Task<SizeResponseDTO> UpdateSizeAsync(SizeDTO sizeDTO)
        {
            var size = await _sizeRepository.GetSizeById(sizeDTO.Id.Value);
            if (size == null)
            {
                return new SizeResponseDTO { Status = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(sizeDTO, size);

            await _sizeRepository.Update(size);
            return new SizeResponseDTO { Status = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
