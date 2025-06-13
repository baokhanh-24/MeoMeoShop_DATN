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
        private readonly IImageRepository _imgRepo;
        public ImageController(IImageRepository imageRepository)
        {
            _imgRepo = imageRepository;
        }
        // GET: api/image
        [HttpGet]
        public async Task<ActionResult<List<ImageDTO>>> GetAll()
        {
            var images = await _imgRepo.GetAllImage();
            var result = images.Select(img => new ImageDTO
            {
                Id = img.Id,
                ProductDetailId = img.ProductDetailId,
                Name = img.Name,
                Type = img.Type,
                UrlImg = img.URL
            }).ToList();

            return Ok(result);
        }
        // GET: api/image/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageDTO>> GetById(Guid id)
        {
            var img = await _imgRepo.GetImageById(id);
            if (img == null) return NotFound();

            var dto = new ImageDTO
            {
                Id = img.Id,
                ProductDetailId = img.ProductDetailId,
                Name = img.Name,
                Type = img.Type,
                UrlImg = img.URL
            };

            return Ok(dto);
        }
        // POST: api/image
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImageDTO imageDto)
        {
            var newImage = new Image
            {
                Id = imageDto.Id ?? Guid.NewGuid(),
                ProductDetailId = imageDto.ProductDetailId,
                Name = imageDto.Name,
                Type = imageDto.Type,
                URL = imageDto.UrlImg
            };

            try
            {
                await _imgRepo.Create(newImage);
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
                var entity = await _imgRepo.GetImageById(id);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                // Kiểm tra productDetailId hợp lệ
                //var productExists = await _imgRepo.CheckProductDetailExist(dto.ProductDetailId);
                //if (!productExists)
                //{
                //    return NotFound($"ProductDetail with ID {dto.ProductDetailId} not found.");
                //}

                // Gán dữ liệu từ DTO sang Entity
                
                entity.Name = dto.Name;
                entity.Type = dto.Type;
                entity.URL = dto.UrlImg;

                await _imgRepo.Update(dto.Id.Value, dto.ProductDetailId);

                return Ok("Update successful.");
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
            await _imgRepo.Delete(id);
            return Ok("Image updated successfully.");
        }
    }
}
