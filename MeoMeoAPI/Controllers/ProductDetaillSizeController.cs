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
            var images = await _productDetaillSizeService.GetAllProductDetaillSizeAsync();
            var result = images.Select(img => new ProductDetaillSizeDTO
            {
                SizeId = img.SizeId,
                ProductDetaillId = img.ProductDetailId,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var img = await _productDetaillSizeService.GetProductDetaillSizeByIdAsync(id);
            if (img == null) return NotFound();

            var dto = new ProductDetaillSizeDTO
            {

                SizeId = id,
                ProductDetaillId = img.ProductDetailId,

            };
            return Ok(dto);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDetaillSizeDTO productDetaillSizeDTO)
        {
            var newItem = new ProductDetailSize
            {
                SizeId = productDetaillSizeDTO.SizeId,
                ProductDetailId = productDetaillSizeDTO.ProductDetaillId,
            };

            try
            {
                await _productDetaillSizeService.CreateProductDetaillSizeAsync(productDetaillSizeDTO);
                return CreatedAtAction(nameof(GetById), new { id = newItem.SizeId }, productDetaillSizeDTO);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDetaillSizeDTO dto)
        {
            if (id != dto.SizeId)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _productDetaillSizeService.UpdateProductDetaillSizeAsync(dto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                await _productDetaillSizeService.UpdateProductDetaillSizeAsync(dto);

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
                var result = await _productDetaillSizeService.DeleteProductDetaillSizeAsync(id);
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
