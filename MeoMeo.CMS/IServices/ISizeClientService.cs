using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Domain.Commons;

namespace MeoMeo.CMS.IServices
{
    public interface ISizeClientService
    {
        Task<PagingExtensions.PagedResult<SizeDTO>> GetAllSizeAsync(GetListSizeRequestDTO request);
        Task<SizeDTO> GetSizeByIdAsync(Guid id);
        Task<SizeResponseDTO> CreateSizeAsync(SizeDTO size);
        Task<SizeResponseDTO> UpdateSizeAsync(SizeDTO size);
        Task<SizeResponseDTO> UpdateSizeStatusAsync(UpdateSizeStatusRequestDTO dto);
        Task<bool> DeleteSizeAsync(Guid id);
    }
}
