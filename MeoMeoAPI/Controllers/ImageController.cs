using MeoMeo.Application.IServices;
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
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        // GET: api/image
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _imageService.GetAllImagesAsync();
            var result = images.Select(img => new ImageDTO
            {
                Id = img.Id,
                ProductDetailId = img.ProductDetailId,
                Name = img.Name,
                Type = img.Type,
                UrlImg = img.URL,
            }).ToList();

            return Ok(result);
        }
        // GET: api/image/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var img = await _imageService.GetImageByIdAsync(id);
            if (img == null) return NotFound();

            var dto = new ImageDTO
            {
                Id = img.Id,
                ProductDetailId = img.ProductDetailId,
                Name = img.Name,
                Type = img.Type,
                UrlImg = img.URL,
            };

            return Ok(dto);
        }
        // POST: api/image
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImageDTO imageDto)
        {
            var newImage = new Image
            {
                ProductDetailId = imageDto.ProductDetailId,
                Name = imageDto.Name,
                Type = imageDto.Type,
                URL = imageDto.UrlImg
            };

            try
            {
                await _imageService.CreateImageAsync(imageDto);
                return CreatedAtAction(nameof(GetById), new { id = newImage.Id }, imageDto);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        // PUT: api/image/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ImageDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _imageService.UpdateImageAsync(dto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }               

                await _imageService.UpdateImageAsync(dto);

                return Ok("Sửa ảnh thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
            // DELETE: api/image/{id}
            [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _imageService.DeleteImageAsync(id);
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
