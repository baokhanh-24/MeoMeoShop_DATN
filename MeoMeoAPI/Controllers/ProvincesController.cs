using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvincesController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvincesController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _provinceService.GetAllProvinceAsync();
            return Ok(result);
        }

        [HttpGet("find-province-by-id{id}")]
        public async Task<IActionResult> GetProvinceById(Guid id)
        {
            var result = await _provinceService.GetProvinceByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-province")]
        public async Task<IActionResult> CreateProvince([FromBody] CreateOrUpdateProvinceDTO provinceDTO)
        {
            var result = await _provinceService.CreateProvinceAsync(provinceDTO);
            return Ok(result);
        }

        [HttpPut("update-province")]
        public async Task<IActionResult> UpdateProvince(Guid id, [FromBody] CreateOrUpdateProvinceDTO provinceDTO)
        {
            var result = await _provinceService.UpdateProvinceAsync(provinceDTO);
            return Ok(result);
        }

        [HttpDelete("delete-province/{id}")]
        public async Task<IActionResult> DeleteProvince(Guid id)
        {
            var result = await _provinceService.DeleteProvinceAsync(id);
            return Ok(result);
        }
    }
}
