using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IBrandServices
    {

        Task<PagingExtensions.PagedResult<BrandDTO>> GetAllBrandsAsync(GetListBrandRequestDTO request);
        Task<BrandDTO> GetBrandByIdAsync(Guid id);
        Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync(CreateOrUpdateBrandDTO brandDto);
        Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(CreateOrUpdateBrandDTO brandDto);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
