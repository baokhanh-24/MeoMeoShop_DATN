using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Contract.Commons;
using MeoMeo.API.Extensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandServices _brandServices;
        private readonly IWebHostEnvironment _environment;

        public BrandsController(IBrandServices brandServices, IWebHostEnvironment environment)
        {
            _brandServices = brandServices;
            _environment = environment;
        }


        [HttpGet("get-all-brand-async")]
        public async Task<PagingExtensions.PagedResult<BrandDTO>> GetAllBrandsAsync([FromQuery] GetListBrandRequestDTO request)
        {
            var result = await _brandServices.GetAllBrandsAsync(request);
            return result;
        }


        [HttpGet("find-brand-by-id-async/{id}")]
        public async Task<BrandDTO> GetBrandByIdAsync(Guid id)
        {

            var result = await _brandServices.GetBrandByIdAsync(id);
            return result;

        }

        [HttpPut("update-brand-async/{id}")]
        public async Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(Guid id, [FromForm] CreateOrUpdateBrandDTO brandDto)
        {
            // Xử lý upload logo nếu có (giống như ProductService)
            List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();

            if (brandDto.LogoFile != null && brandDto.LogoFile.Length > 0)
            {
                var filesToUpload = new List<IFormFile> { brandDto.LogoFile };
                uploadedFiles = await FileUploadHelper.UploadFilesAsync(_environment, filesToUpload, "Brands", id);
            }

            var result = await _brandServices.UpdateBrandAsync(brandDto, uploadedFiles);

            if (result.ResponseStatus == BaseStatus.Error)
            {
                // Rollback uploaded files if service failed
                FileUploadHelper.DeleteUploadedFiles(_environment, uploadedFiles);
            }

            return result;
        }


        [HttpPost("create-brand-async")]
        public async Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync([FromForm] CreateOrUpdateBrandDTO brandDto)
        {
            // Xử lý upload logo nếu có (giống như ProductService)
            List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();

            if (brandDto.LogoFile != null && brandDto.LogoFile.Length > 0)
            {
                var brandId = Guid.NewGuid();
                var filesToUpload = new List<IFormFile> { brandDto.LogoFile };
                uploadedFiles = await FileUploadHelper.UploadFilesAsync(_environment, filesToUpload, "Brands", brandId);
                brandDto.Id = brandId;
            }

            var result = await _brandServices.CreateBrandAsync(brandDto, uploadedFiles);

            if (result.ResponseStatus == BaseStatus.Error)
            {
                // Rollback uploaded files if service failed
                FileUploadHelper.DeleteUploadedFiles(_environment, uploadedFiles);
            }

            return result;
        }


        [HttpDelete("delete-brand-async/{id}")]
        public async Task<bool> DeleteBrandAsync(Guid id)
        {

            var result = await _brandServices.DeleteBrandAsync(id);
            return result;
        }

    }
}
