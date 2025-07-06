using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        Task<List<Brand>> GetAllBrandsAsync();
        Task<Brand> GetBrandByIdAsync(Guid id);
        Task<Brand> CreateBrandAsync(Brand brand);
        Task<Brand> UpdateBrandAsync(Brand brand);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
