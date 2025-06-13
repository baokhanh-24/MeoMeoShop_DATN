using MeoMeo.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IBrandServices
    {
        Task<List<BrandDTO>> GetAllBrandsAsync();
        Task<BrandDTO> GetBrandByIdAsync(Guid id);
        Task<BrandDTO> CreateBrandAsync(BrandDTO brandDto);
        Task<BrandDTO> UpdateBrandAsync(Guid id, BrandDTO brandDto);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
