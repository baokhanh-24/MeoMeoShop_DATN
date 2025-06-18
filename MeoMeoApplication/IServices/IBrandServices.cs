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
        Task<Brand> GetBrandByIdAsync(Guid id);
        Task<Brand> CreateBrandAsync(BrandDTO brandDto);
        Task<Brand> UpdateBrandAsync(Guid id, BrandDTO brandDto);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
