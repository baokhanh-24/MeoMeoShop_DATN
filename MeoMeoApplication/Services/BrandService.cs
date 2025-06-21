using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        async Task<CreateOrUpdateBrandResponse> IBrandServices.CreateBrandAsync(BrandDTO brandDto)
        {
            CreateOrUpdateBrandResponse response = new CreateOrUpdateBrandResponse();
            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = brandDto.Name,
                Code = brandDto.Code,
                EstablishYear = brandDto.EstablishYear,
                Country = brandDto.Country,
                Description = brandDto.Description,
                Logo = brandDto.Logo
            };
            await _brandRepository.CreateBrandAsync(brand);
            response.Id = brand.Id;
            response.Name = brand.Name;
            response.Code = brand.Code;

            response.EstablishYear = brand.EstablishYear;
            response.Country = brand.Country;
            response.Description = brand.Description;
            response.Logo = brand.Logo;
            response.Message = "Tạo thương hiệu thành công";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateBrandResponse> IBrandServices.GetBrandByIdAsync(Guid id)
        {
            CreateOrUpdateBrandResponse response = new CreateOrUpdateBrandResponse();
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            if (brand == null)
            {
                response.Message = "Brand not found";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            response.Id = brand.Id;
            response.Name = brand.Name;
            response.Code = brand.Code;
            response.EstablishYear = brand.EstablishYear;
            response.Country = brand.Country;
            response.Description = brand.Description;
            response.Logo = brand.Logo;
            response.Message = "Lấy thương hiệu thành công";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateBrandResponse> IBrandServices.UpdateBrandAsync(Guid id, BrandDTO brandDto)
        {
            CreateOrUpdateBrandResponse response = new CreateOrUpdateBrandResponse();
            var getbran = await _brandRepository.GetBrandByIdAsync(id);
            if (getbran == null)
            {
                response.Message = "Không tìm thấy thương hiệu này";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }

            getbran.Name = brandDto.Name;
            getbran.Code = brandDto.Code;
            getbran.EstablishYear = brandDto.EstablishYear;
            getbran.Country = brandDto.Country;
            getbran.Description = brandDto.Description;
            getbran.Logo = brandDto.Logo;
            await _brandRepository.UpdateBrandAsync(id, getbran);
            response.Message = "Cập nhật thương hiệu thành công";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
