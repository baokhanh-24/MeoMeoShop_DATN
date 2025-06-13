using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class BrandService : IBrandServices
    {
        private readonly MeoMeoDbContext _context;
        public BrandService(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task<BrandDTO> CreateBrandAsync(BrandDTO brandDto)
        {
            var newBrand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = brandDto.Name,
                Code = brandDto.Code,
                EstablishYear = brandDto.EstablishYear,
                Country = brandDto.Country,
                Description = brandDto.Description,
                Logo = brandDto.Logo
            };
            _context.brands.Add(newBrand);
            await _context.SaveChangesAsync();
            return new BrandDTO
            {
                Id = newBrand.Id,
                Name = newBrand.Name,
                Code = newBrand.Code,
                EstablishYear = newBrand.EstablishYear,
                Country = newBrand.Country,
                Description = newBrand.Description,
                Logo = newBrand.Logo
            };
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            var phat = await _context.brands.FindAsync(id);
            if (phat == null)
            {
                return false;
            }
            _context.brands.Remove(phat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BrandDTO>> GetAllBrandsAsync()
        {
            return await _context.brands.Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    EstablishYear = b.EstablishYear,
                    Country = b.Country,
                    Description = b.Description,
                    Logo = b.Logo
                }).ToListAsync();
        }

        public async Task<BrandDTO> GetBrandByIdAsync(Guid id)
        {
            var phat = await _context.brands.FindAsync(id);
            if (phat == null)
            {
                return null;
            }
            return new BrandDTO
            {
                Id = phat.Id,
                Name = phat.Name,
                Code = phat.Code,
                EstablishYear = phat.EstablishYear,
                Country = phat.Country,
                Description = phat.Description,
                Logo = phat.Logo
            };
        }

        public async Task<BrandDTO> UpdateBrandAsync(Guid id, BrandDTO brandDto)
        {
            var phat = await _context.brands.FindAsync(id);
            if (phat == null)
            {
                return null;
            }
            phat.Name = brandDto.Name;
            phat.Code = brandDto.Code;
            phat.EstablishYear = brandDto.EstablishYear;
            phat.Country = brandDto.Country;
            phat.Description = brandDto.Description;
            phat.Logo = brandDto.Logo;
            _context.brands.Update(phat);
            await _context.SaveChangesAsync();
            return new BrandDTO
            {
                Id = phat.Id,
                Name = phat.Name,
                Code = phat.Code,
                EstablishYear = phat.EstablishYear,
                Country = phat.Country,
                Description = phat.Description,
                Logo = phat.Logo
            };
        }
    }
}
