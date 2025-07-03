using AutoMapper;
using Azure.Core;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
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
                Code = sizeDTO.Code,
                Status = sizeDTO.Status,
               
            };
            var updated = await _sizeRepository.Create(size);
            return new SizeResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public async Task<SizeResponseDTO> DeleteSizeAsync(Guid id)
        {
            var size = await _sizeRepository.GetSizeById(id);
            if (size == null)
            {
                return new SizeResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }

            await _sizeRepository.Delete(id);
            return new SizeResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Xóa thành công" };
        }

        public async Task<PagingExtensions.PagedResult<SizeDTO>> GetAllSizeAsync(GetListSizeRequestDTO dto)
        {
            try
            {
                var query = _sizeRepository.Query();
                if (!string.IsNullOrEmpty(dto.ValueFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Value, $"%{dto.ValueFilter}%"));
                }
                if (!string.IsNullOrEmpty(dto.CodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Code, $"%{dto.CodeFilter}%"));
                }
                if (dto.StatusFilter != null)
                {
                    query = query.Where(c => c.Status == (int)dto.StatusFilter);
                }

                query = query.OrderByDescending(c => c.Value);
                var filteredSize = await _sizeRepository.GetPagedAsync(query, dto.PageIndex, dto.PageSize);
                var dtoItems = _mapper.Map<List<SizeDTO>>(filteredSize.Items);

                return new PagingExtensions.PagedResult<SizeDTO>
                {
                    TotalRecords = filteredSize.TotalRecords,
                    PageIndex = filteredSize.PageIndex,
                    PageSize = filteredSize.PageSize,
                    Items = dtoItems
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllSizeAsync: {ex.Message}");
                throw;
            }
        }


        public async Task<SizeResponseDTO> GetSizeByIdAsync(Guid id)
        {
            var size = await _sizeRepository.GetSizeById(id);
            if(size == null)
            {
                return new SizeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy Size với ID: {id}"
                };
            }
            return new SizeResponseDTO 
            { 
                Id = size.Id,
                Value = size.Value,
                Code = size.Code,
                Status = size.Status,
                ResponseStatus = BaseStatus.Success,
                Message = $""
            };
        }

        public async Task<SizeResponseDTO> UpdateSizeAsync(SizeDTO sizeDTO)
        {
            var size = await _sizeRepository.GetSizeById(sizeDTO.Id);
            if (size == null)
            {
                return new SizeResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(sizeDTO, size);

            await _sizeRepository.Update(size);
            return new SizeResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
