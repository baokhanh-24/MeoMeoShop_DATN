
using AutoMapper;
using MeoMeo.Application.IServices;
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
                    query = query.Where(x => x.Name.Contains(request.NameFilter));
                }
                if (!string.IsNullOrWhiteSpace(request.CountryFilter))
                {
                    query = query.Where(x => x.Country.Contains(request.CountryFilter));
                }
                if (!string.IsNullOrWhiteSpace(request.CodeFilter))
                {
                    query = query.Where(x => x.Code.Contains(request.CodeFilter));
                }
                if (request.EstablishYearFilter.HasValue)
                {
                    query = query.Where(x => x.EstablishYear.Year == request.EstablishYearFilter.Value);
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

        public async Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(CreateOrUpdateBrandDTO brandDto, List<FileUploadResult>? uploadedFiles = null)
        {
            // Check trùng Name (trừ record hiện tại)
            var isNameExist = await _brandRepository.AnyAsync(x => x.Name == brandDto.Name && x.Id != brandDto.Id);
            if (isNameExist)
            {
                return new CreateOrUpdateBrandResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tên thương hiệu đã tồn tại."
                };
            }

            var brandToUpdate = await _brandRepository.GetBrandByIdAsync(brandDto.Id.Value);
            if (brandToUpdate == null)
            {
                return new CreateOrUpdateBrandResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy thương hiệu cần cập nhật."
                };
            }

            // Giữ nguyên Code, chỉ cập nhật các trường khác
            var originalCode = brandToUpdate.Code;
            _mapper.Map(brandDto, brandToUpdate);
            brandToUpdate.Code = originalCode; // Giữ nguyên Code cũ

            // Xử lý logo từ uploadedFiles (giống như ProductService)
            if (uploadedFiles != null && uploadedFiles.Any())
            {
                var logoFile = uploadedFiles.FirstOrDefault();
                if (logoFile != null)
                {
                    brandToUpdate.Logo = $"/{logoFile.RelativePath}";
                }
            }

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


        public async Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync(CreateOrUpdateBrandDTO brandDto, List<FileUploadResult>? uploadedFiles = null)

        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check trùng Name
                var isNameExist = await _brandRepository.AnyAsync(x => x.Name == brandDto.Name);
                if (isNameExist)
                {
                    return new CreateOrUpdateBrandResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Tên thương hiệu đã tồn tại."
                    };
                }

                // Tự động generate Code
                var brandCode = await GenerateBrandCodeAsync();


                var brand = _mapper.Map<Brand>(brandDto);
                brand.Id = Guid.NewGuid();
                brand.Code = brandCode;

                // Xử lý logo từ uploadedFiles (giống như ProductService)
                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    var logoFile = uploadedFiles.FirstOrDefault();
                    if (logoFile != null)
                    {
                        brand.Logo = $"/{logoFile.RelativePath}";
                    }
                }

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

        private async Task<string> GenerateBrandCodeAsync()
        {
            var lastBrand = await _brandRepository.Query()
                .OrderByDescending(x => x.Code)
                .FirstOrDefaultAsync();

            if (lastBrand == null || string.IsNullOrEmpty(lastBrand.Code))
            {
                return "BRAND001";
            }

            // Extract number from last code (e.g., "BRAND001" -> 1)
            var lastCode = lastBrand.Code;
            if (lastCode.StartsWith("BRAND"))
            {
                var numberPart = lastCode.Substring(5); // Remove "BRAND"
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    return $"BRAND{(lastNumber + 1):D3}";
                }
            }

            // Fallback: count existing brands
            var count = await _brandRepository.Query().CountAsync();
            return $"BRAND{(count + 1):D3}";
        }

    }
}
