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
        private readonly IProductDetaillColourRepository _productDetaillColourRepository;
        public ProductDetaillColourController(IProductDetaillColourRepository productDetaillColourRepository)
        {
            _productDetaillColourRepository = productDetaillColourRepository;
        }
        //
        [HttpGet]
        public async Task<ActionResult<List<ProductDetaillColourDTO>>> GetAll()
        {
            var images = await _productDetaillColourRepository.GetAllProductDetaillColour();
            var result = images.Select(img => new ProductDetaillColourDTO
            {
                ColourId = img.ColourId,
                ProductDetaillId = img.ProductDetailId,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetaillColourDTO>> GetById(Guid id)
        {
            var img = await _productDetaillColourRepository.GetProductDetaillColourById(id);
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
                await _productDetaillColourRepository.Create(newImage);
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
                var entity = await _productDetaillColourRepository.GetProductDetaillColourById(id);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }              
                // Gán dữ liệu từ DTO sang Entity

                entity.ColourId = dto.ColourId;
                entity.ProductDetailId = dto.ProductDetaillId;

                await _productDetaillColourRepository.Update(dto.ColourId, dto.ProductDetaillId);

                return Ok("Update successful.");
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
            await _productDetaillColourRepository.Delete(id);
            return Ok("Xoa thanh cong.");
        }
    }
}
