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
    public class ColourController : ControllerBase
    {
        private readonly IColourService _colourService;
        public ColourController(IColourService colourService)
        {
            _colourService = colourService;
        }
        //
        [HttpGet]
        public async Task<IActionResult>GetAll()
        {
            var colour = await _colourService.GetAllColoursAsync();
            var result = colour.Select(img => new ColourDTO
            {
                Id = img.Id,
                Name = img.Name,
                Code = img.Code,
                Status = img.Status,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var img = await _colourService.GetColourByIdAsync(id);
            if (img == null) return NotFound();

            var dto = new ColourDTO
            {
                Id = img.Id,
                Name = img.Name,
                Code = img.Code,
                Status = img.Status,
            };

            return Ok(dto);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ColourDTO colourDTO)
        {
            var newColour = new Colour
            {
                Name = colourDTO.Name,
                Code = colourDTO.Code,
                Status = colourDTO.Status,
            };
            try
            {
                await _colourService.CreateColourAsync(colourDTO);
                return CreatedAtAction(nameof(GetById), new { id = newColour.Id }, colourDTO);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ColourDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _colourService.UpdateColourAsync(dto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                await _colourService.UpdateColourAsync(dto);

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
                var result = await _colourService.DeleteColourAsync(id);
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
