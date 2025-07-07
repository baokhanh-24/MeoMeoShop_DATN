
﻿using AutoMapper;
using MeoMeo.Application.IServices;
﻿using MeoMeo.Application.IServices;

using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class BrandService : IBrandServices
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IBrandRepository brandRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


       public async Task<PagingExtensions.PagedResult<BrandDTO>> GetAllBrandsAsync(GetListBrandRequestDTO request)
{
    try
    {
        var query = _brandRepository.Query();

        if (!string.IsNullOrWhiteSpace(request.NameFilter))
        {
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.NameFilter}%"));
        }
        if (!string.IsNullOrWhiteSpace(request.CountryFilter))
        {
            query = query.Where(x => EF.Functions.Like(x.Country, $"%{request.CountryFilter}%"));
        }
        if (!string.IsNullOrWhiteSpace(request.CodeFilter))
        {
            query = query.Where(x => EF.Functions.Like(x.Code, $"%{request.CodeFilter}%"));
        }
        if (request.EstablishYearFilter.HasValue)
        {
            query = query.Where(x => x.EstablishYear.Year == request.EstablishYearFilter.Value); // 🔁 Sửa chỗ này
        }

        query = query.OrderByDescending(x => x.Name);
        var pagedResult = await _brandRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);
        var dtoItems = _mapper.Map<List<BrandDTO>>(pagedResult.Items);

        return new PagingExtensions.PagedResult<BrandDTO>
        {
            TotalRecords = pagedResult.TotalRecords,
            PageIndex = pagedResult.PageIndex,
            PageSize = pagedResult.PageSize,
            Items = dtoItems
        };
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}


        public async Task<BrandDTO> GetBrandByIdAsync(Guid id)
        {
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            return _mapper.Map<BrandDTO>(brand);
        }

        public async Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(CreateOrUpdateBrandDTO brandDto)
        {
            var brandExists = await _brandRepository.AnyAsync(c => c.Code == brandDto.Code && c.Id != brandDto.Id);
            if (brandExists)
            {
                return new CreateOrUpdateBrandResponseDTO()
                {
                    Message = $"Code đã tồn tại",
                    ResponseStatus = BaseStatus.Error,
                };
            }

            var brandToUpdate = await _brandRepository.GetBrandByIdAsync(brandDto.Id);
            _mapper.Map(brandDto, brandToUpdate);

            var result = await _brandRepository.UpdateBrandAsync(brandToUpdate);
            return _mapper.Map<CreateOrUpdateBrandResponseDTO>(result);
        }


        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return false;
            }
            await _brandRepository.DeleteBrandAsync(brand.Id);
            return true;
        }


        public async Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync(CreateOrUpdateBrandDTO brandDto)

        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var isCodeExist = await _brandRepository.AnyAsync(x => x.Code == brandDto.Code);
                if (isCodeExist)
                {
                    return new CreateOrUpdateBrandResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Mã code đã tồn tại."
                    };
                }


                var brand = _mapper.Map<Brand>(brandDto);
                brand.Id = Guid.NewGuid();

                var createdBrand = await _brandRepository.CreateBrandAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                var response = _mapper.Map<CreateOrUpdateBrandResponseDTO>(createdBrand);
                response.ResponseStatus = BaseStatus.Success;
                response.Message = "Tạo brand thành công";
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Lỗi khi tạo brand: {ex.Message}");
                return new CreateOrUpdateBrandResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message,
                };
            }
        }

      



      
    }
}
