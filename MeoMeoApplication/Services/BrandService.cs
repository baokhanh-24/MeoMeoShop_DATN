using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
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
        private readonly IBrandRepository _brandRepository;
        public BrandService(IBrandRepository context)
        {
            _brandRepository = context;
        }

        public Task<Brand> CreateBrandAsync(BrandDTO brandDto)
        {
            var phat = new Brand
            {
                Id = Guid.NewGuid(),
                Name = brandDto.Name,
                Code = brandDto.Code,
                EstablishYear = brandDto.EstablishYear,
                Country = brandDto.Country,
                Description = brandDto.Description,
                Logo = brandDto.Logo
            };
            return _brandRepository.CreateBrandAsync(phat);
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            var phat = await _brandRepository.GetBrandByIdAsync(id);
            if (phat == null)
            {
                return false; 
            }
            return await _brandRepository.DeleteBrandAsync(id);

        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await _brandRepository.GetAllBrandsAsync();
        }

        public async Task<Brand> GetBrandByIdAsync(Guid id)
        {
            return await _brandRepository.GetBrandByIdAsync(id);
        }

        public async Task<Brand> UpdateBrandAsync(Guid id, BrandDTO brandDto)
        {
            var phat = await _brandRepository.GetBrandByIdAsync(id);
            if (phat == null)
            {
                return null; // or throw an exception
            }
            phat.Name = brandDto.Name;
            phat.Code = brandDto.Code;
            phat.EstablishYear = brandDto.EstablishYear;
            phat.Country = brandDto.Country;
            phat.Description = brandDto.Description;
            phat.Logo = brandDto.Logo;
            return await _brandRepository.UpdateBrandAsync(id, phat);
        }
    }
}
