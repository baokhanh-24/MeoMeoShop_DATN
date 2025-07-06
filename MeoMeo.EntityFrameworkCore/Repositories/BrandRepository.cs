using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public BrandRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Brand> CreateBrandAsync(Brand brand)
        {
           var brandAdd = await AddAsync(brand);
            return brandAdd;
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
           var brandAll = await GetAllAsync();
            return brandAll.ToList();
        }

        public async Task<Brand> GetBrandByIdAsync(Guid id)
        {
            var brandId = await GetByIdAsync(id);
            return brandId;
        }

        public async Task<Brand> UpdateBrandAsync(Brand brand)
        {
            var brandUpdate = await UpdateAsync(brand);
            return brandUpdate;
        }
    }
}
