using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetaillColourController : ControllerBase
    {
        private readonly IProductDetaillColourService _productDetaillColourService;
        public ProductDetaillColourController(IProductDetaillColourService productDetaillColourService)
        {
            _productDetaillColourService = productDetaillColourService;
        }
        //
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productDetaillColourService.GetAllProductDetaillColourAsync();
            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productDetaillColourService.GetProductDetaillColourByIdAsync(id);
            return Ok(result);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDetaillColourDTO productDetaillColourDTO)
        {
            var result = await _productDetaillColourService.CreateProductDetaillColourAsync(productDetaillColourDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDetaillColourDTO dto)
        {
            var result = await _productDetaillColourService.UpdateProductDetaillColourAsync(dto);
            return Ok(result);
        }
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productDetaillColourService.DeleteProduuctDetaillColourAsync(id);
            return Ok(result);
        }
    }
}
