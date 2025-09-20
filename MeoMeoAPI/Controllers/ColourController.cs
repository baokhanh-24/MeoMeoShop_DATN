using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.Domain.Commons;
using static MeoMeo.Domain.Commons.PagingExtensions;

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
        public async Task<IActionResult> GetAll()
        {
            var result = await _colourService.GetAllColoursAsync();
            return Ok(result);
        }

        //
        [HttpGet("get-all-colours-paged")]
        public async Task<PagedResult<ColourDTO>> GetAllColoursPaged([FromQuery] GetListColourRequest request)
        {
            var result = await _colourService.GetAllColoursPagedAsync(request);
            return result;
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _colourService.GetColourByIdAsync(id);
            return Ok(result);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ColourDTO colourDTO)
        {
            var result = await _colourService.CreateColourAsync(colourDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ColourDTO dto)
        {
            var result = await _colourService.UpdateColourAsync(dto);
            return Ok(result);
        }
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _colourService.DeleteColourAsync(id);
            return Ok(result);
        }
    }
}
