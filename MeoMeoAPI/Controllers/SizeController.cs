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
    public class SizeController : ControllerBase
    {
        private readonly ISizeService _sizeService;
        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }
        //
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var size = await _sizeService.GetAllSizeAsync();
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
        public async Task<IActionResult> GetById(Guid id)
        {
            var img = await _sizeService.GetSizeByIdAsync(id);
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
                await _sizeService.CreateSizeAsync(sizeDTO);
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
                var entity = await _sizeService.UpdateSizeAsync(dto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                await _sizeService.UpdateSizeAsync(dto);

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
                var result = await _sizeService.DeleteSizeAsync(id);
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
