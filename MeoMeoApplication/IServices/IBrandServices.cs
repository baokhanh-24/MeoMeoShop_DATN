using MeoMeo.Contract.DTOs;
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
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
        Task<CreateOrUpdateBrandResponse> GetBrandByIdAsync(Guid id);
        Task<CreateOrUpdateBrandResponse> CreateBrandAsync(BrandDTO brandDto);
        Task<CreateOrUpdateBrandResponse> UpdateBrandAsync(Guid id, BrandDTO brandDto);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
