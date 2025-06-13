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
        private readonly IColourRepository _IColourRepo;
        public ColourController(IColourRepository colourRepository)
        {
            _IColourRepo = colourRepository;
        }
        //
        [HttpGet]
        public async Task<ActionResult<List<ColourDTO>>> GetAll()
        {
            var colour = await _IColourRepo.GetAllColour();
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
        public async Task<ActionResult<ColourDTO>> GetById(Guid id)
        {
            var img = await _IColourRepo.GetColourById(id);
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
                Id = colourDTO.Id ?? Guid.NewGuid(),
                Name = colourDTO.Name,
                Code = colourDTO.Code,
                Status = colourDTO.Status,
            };
            try
            {
                await _IColourRepo.Create(newColour);
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
                var entity = await _IColourRepo.GetColourById(id);
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
                entity.Code = dto.Code;
                entity.Status = dto.Status;

                await _IColourRepo.Update(dto.Id.Value);

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
            await _IColourRepo.Delete(id);
            return Ok("Image updated successfully.");
        }
    }
}
