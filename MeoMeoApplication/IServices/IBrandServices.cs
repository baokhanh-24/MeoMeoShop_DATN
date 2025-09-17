using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Contract.Commons;
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
        Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync(CreateOrUpdateBrandDTO brandDto, List<FileUploadResult>? uploadedFiles = null);
        Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(CreateOrUpdateBrandDTO brandDto, List<FileUploadResult>? uploadedFiles = null);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
