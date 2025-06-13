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
        private readonly IProductDetaillSizeRepository _productDetaillSizeRepo;
        public ProductDetaillSizeController(IProductDetaillSizeRepository productDetaillSizeRepository)
        {
            _productDetaillSizeRepo = productDetaillSizeRepository;
        }
        //
        [HttpGet]
        public async Task<ActionResult<List<ProductDetaillSizeDTO>>> GetAll()
        {
            var images = await _productDetaillSizeRepo.GetAllProductDetaillSize();
            var result = images.Select(img => new ProductDetaillSizeDTO
            {
                SizeId = img.SizeId,
                ProductDetaillId = img.ProductDetailId,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetaillSizeDTO>> GetById(Guid id)
        {
            var img = await _productDetaillSizeRepo.GetProductDetaillSizeById(id);
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
                await _productDetaillSizeRepo.Create(newItem);
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
                var entity = await _productDetaillSizeRepo.GetProductDetaillSizeById(id);
                if (entity == null)
                {
                    return NotFound($"ProductDetaillSize with ID {id} not found.");
                }

                // Gán dữ liệu từ DTO sang Entity

                entity.SizeId = dto.SizeId;
                entity.ProductDetailId = dto.ProductDetaillId;

                await _productDetaillSizeRepo.Update(dto.SizeId, dto.ProductDetaillId);

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
            await _productDetaillSizeRepo.Delete(id);
            return Ok("Xoa thanh cong.");
        }
    }
}
