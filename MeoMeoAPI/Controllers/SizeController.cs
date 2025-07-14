using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MeoMeo.Domain.Commons.PagingExtensions;

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
        public async Task<ActionResult<PagedResult<SizeDTO>>> GetAllSizes([FromQuery] GetListSizeRequestDTO request)
        {
            var result = await _sizeService.GetAllSizeAsync(request);
            return Ok(result);
        }

        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sizeService.GetSizeByIdAsync(id);
            return Ok(result);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SizeDTO sizeDTO)
        {
            var result = await _sizeService.CreateSizeAsync(sizeDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SizeDTO dto)
        {
            dto.Id = id;
            var result = await _sizeService.UpdateSizeAsync(dto);
            return Ok(result);
        }
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sizeService.DeleteSizeAsync(id);
            return Ok(result);
        }
        [HttpPut("update-size-status")]
        public async Task<IActionResult> UpdateSizeStatus([FromBody] UpdateSizeStatusRequestDTO dto)
        {
            var result = await _sizeService.UpdateSizeStatusAsync(dto);
            return Ok(result);
        }
    }
}
