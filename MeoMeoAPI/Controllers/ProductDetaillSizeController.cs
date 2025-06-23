using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetaillSizeController : ControllerBase
    {
        private readonly IProductDetaillSizeService _productDetaillSizeService;
        public ProductDetaillSizeController(IProductDetaillSizeService productDetaillSizeService)
        {
            _productDetaillSizeService = productDetaillSizeService;
        }
        //
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productDetaillSizeService.GetAllProductDetaillSizeAsync();
            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productDetaillSizeService.GetProductDetaillSizeByIdAsync(id);
            return Ok(result);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDetaillSizeDTO productDetaillSizeDTO)
        {
            var result = await _productDetaillSizeService.CreateProductDetaillSizeAsync(productDetaillSizeDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDetaillSizeDTO dto)
        {
            var result = await _productDetaillSizeService.UpdateProductDetaillSizeAsync(dto);
            return Ok(result);
        }
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productDetaillSizeService.DeleteProductDetaillSizeAsync(id);
            return Ok(result);
        }
    }
}
