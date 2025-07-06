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

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandServices _brandServices;

        public BrandsController(IBrandServices brandServices)
        {
            _brandServices = brandServices;
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
        public async Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(Guid id, [FromBody] CreateOrUpdateBrandDTO brandDto)
        {
            var result = await _brandServices.UpdateBrandAsync(brandDto);
            return result;
        }


        [HttpPost("create-brand-async")]
        public async Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync([FromBody] CreateOrUpdateBrandDTO brandDto)
        {
            var result = await _brandServices.CreateBrandAsync(brandDto);
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
