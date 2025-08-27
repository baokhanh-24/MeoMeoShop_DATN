using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunesController : ControllerBase
    {
        private readonly ICommuneService _communeService;

        public CommunesController(ICommuneService communeService)
        {
            _communeService = communeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _communeService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("by-district/{districtId}")]
        public async Task<IActionResult> GetByDistrictId(Guid districtId)
        {
            var result = await _communeService.GetByDistrictIdAsync(districtId);
            return Ok(result);
        }
    }
}
