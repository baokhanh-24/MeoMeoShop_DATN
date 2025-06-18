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
            var images = await _productDetaillColourService.GetAllProductDetaillColourAsync();
            var result = images.Select(img => new ProductDetaillColourDTO
            {
                ColourId = img.ColourId,
                ProductDetaillId = img.ProductDetailId,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var img = await _productDetaillColourService.GetProductDetaillColourByIdAsync(id);
            if (img == null) return NotFound();

            var dto = new ProductDetaillColourDTO
            {
                
                ColourId=img.ColourId,
                ProductDetaillId=img.ProductDetailId,
                
            };

            return Ok(dto);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDetaillColourDTO productDetaillColourDTO)
        {
            var newImage = new ProductDetailColour
            {
                ColourId = productDetaillColourDTO.ColourId,
                ProductDetailId = productDetaillColourDTO.ProductDetaillId,
            };

            try
            {
                await _productDetaillColourService.CreateProductDetaillColourAsync(productDetaillColourDTO);
                return CreatedAtAction(nameof(GetById), new { id = newImage.ColourId }, productDetaillColourDTO);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDetaillColourDTO dto)
        {
            if (id != dto.ColourId)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _productDetaillColourService.UpdateProductDetaillColourAsync(dto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                await _productDetaillColourService.UpdateProductDetaillColourAsync(dto);

                return Ok("Sửa ảnh thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _productDetaillColourService.DeleteProduuctDetaillColourAsync(id);
                return Ok(new { message = "Xóa ảnh thành công", result });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa ảnh", detail = ex.Message });
            }
        }
    }
}
