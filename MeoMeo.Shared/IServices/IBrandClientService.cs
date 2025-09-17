using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using Microsoft.AspNetCore.Http;
namespace MeoMeo.Shared.IServices
{
    public interface IBrandClientService
    {
        Task<PagingExtensions.PagedResult<BrandDTO>> GetAllBrandAsync(GetListBrandRequestDTO request);
        Task<BrandDTO> GetBrandByIdAsync(Guid id);
        Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync(CreateOrUpdateBrandDTO brandDto);
        Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(CreateOrUpdateBrandDTO brandDto);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
