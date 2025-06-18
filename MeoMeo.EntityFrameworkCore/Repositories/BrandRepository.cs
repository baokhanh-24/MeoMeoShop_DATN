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
            try
            {
                await AddAsync(brand);
                return brand;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while creating the brand.", ex);
            }
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            var phat = await GetByIdAsync(id);
            if (phat == null)
            {
                return false; 
            }
            await base.DeleteAsync(id);
            return true;

        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Brand> GetBrandByIdAsync(Guid id)
        {
            var phat = await GetByIdAsync(id);
            return phat;
        }

        public async Task<Brand> UpdateBrandAsync(Guid id, Brand brand)
        {
            var phat = await GetByIdAsync(id);
            if (phat == null)
            {
                throw new KeyNotFoundException($"Brand with ID {id} not found.");
            }
            phat.Name = brand.Name;
            phat.Code = brand.Code;
            phat.EstablishYear = brand.EstablishYear;
            phat.Country = brand.Country;
            phat.Description = brand.Description;
            phat.Logo = brand.Logo;
            return await UpdateAsync(phat);
        }
    }
}
