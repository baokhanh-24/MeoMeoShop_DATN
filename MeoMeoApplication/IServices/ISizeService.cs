using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ISizeService
    {
        Task<PagingExtensions.PagedResult<SizeDTO>> GetAllSizeAsync(GetListSizeRequestDTO request);
        Task<SizeResponseDTO> GetSizeByIdAsync(Guid id);
        Task<SizeResponseDTO> CreateSizeAsync(SizeDTO sizeDTO);
        Task<SizeResponseDTO> UpdateSizeAsync(SizeDTO sizeDTO);
        Task<SizeResponseDTO> UpdateSizeStatusAsync(UpdateSizeStatusRequestDTO dto);
        Task<SizeResponseDTO> DeleteSizeAsync(Guid id);
    }
}
