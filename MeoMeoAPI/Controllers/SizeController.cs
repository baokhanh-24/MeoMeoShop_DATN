using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeRepository _sizeRepo;
        public SizeController(ISizeRepository sizeRepository)
        {
            _sizeRepo = sizeRepository;
        }
        //
        [HttpGet]
        public async Task<ActionResult<List<SizeDTO>>> GetAll()
        {
            var size = await _sizeRepo.GetAllSize();
            var result = size.Select(img => new SizeDTO
            {
                Id = img.Id,
                Value = img.Value,
                Code = img.Code,
                Status = img.Status,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<SizeDTO>> GetById(Guid id)
        {
            var img = await _sizeRepo.GetSizeById(id);
            if (img == null) return NotFound();

            var dto = new SizeDTO
            {
                Id = img.Id,
                Value = img.Value,
                Code = img.Code,
                Status = img.Status,
            };

            return Ok(dto);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SizeDTO sizeDTO)
        {
            var newSize = new Size
            {
                Id = sizeDTO.Id ?? Guid.NewGuid(),
                Value = sizeDTO.Value,
                Code = sizeDTO.Code,
                Status = sizeDTO.Status,
            };
            try
            {
                await _sizeRepo.Create(newSize);
                return CreatedAtAction(nameof(GetById), new { id = newSize.Id }, sizeDTO);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SizeDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _sizeRepo.GetSizeById(id);
                if (entity == null)
                {
                    return NotFound($"Size with ID {id} not found.");
                }

                // Kiểm tra productDetailId hợp lệ
                //var productExists = await _imgRepo.CheckProductDetailExist(dto.ProductDetailId);
                //if (!productExists)
                //{
                //    return NotFound($"ProductDetail with ID {dto.ProductDetailId} not found.");
                //}

                // Gán dữ liệu từ DTO sang Entity

                entity.Value = dto.Value;
                entity.Code = dto.Code;
                entity.Status = dto.Status;

                await _sizeRepo.Update(dto.Id.Value);

                return Ok("Update Thành công.");
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
            await _sizeRepo.Delete(id);
            return Ok("Xóa Size thành công.");
        }
    }
}
