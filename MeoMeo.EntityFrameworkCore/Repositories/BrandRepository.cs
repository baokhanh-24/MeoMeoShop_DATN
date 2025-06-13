using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly MeoMeoDbContext _context;
        public BrandRepository(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task<Brand> CreateBrandAsync(Brand brand)
        {
            try
            {
                _context.brands.Add(brand);
                await _context.SaveChangesAsync();
                return brand;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the brand.", ex);
            }
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            try
            {
                var phat = await _context.brands.FindAsync(id);
                if (phat != null)
                {
                    _context.brands.Remove(phat);
                    await _context.SaveChangesAsync();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the brand.", ex);
            }

        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _context.brands.ToListAsync();
        }

        public async Task<Brand> GetBrandByIdAsync(Guid id)
        {
            return await _context.brands.FindAsync(id);
        }

        public async Task<Brand> UpdateBrandAsync(Guid id, Brand brand)
        {
            try
            {
                var existingBrand = await _context.brands.FindAsync(id);
                if (existingBrand == null)
                {
                    throw new Exception("Brand not found.");
                }
                existingBrand.Name = brand.Name;
                existingBrand.Code = brand.Code;
                existingBrand.EstablishYear = brand.EstablishYear;
                existingBrand.Country = brand.Country;
                existingBrand.Description = brand.Description;
                existingBrand.Logo = brand.Logo;
                _context.brands.Update(existingBrand);
                await _context.SaveChangesAsync();
                return existingBrand;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the brand.", ex);
            }
        }
    }
}
