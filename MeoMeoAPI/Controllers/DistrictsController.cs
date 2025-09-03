using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictsController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet("find-district-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _districtService.GetDistrictByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _districtService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("by-province/{provinceId}")]
        public async Task<IActionResult> GetByProvinceId(Guid provinceId)
        {
            var result = await _districtService.GetByProvinceIdAsync(provinceId);
            return Ok(result);
        }
        [HttpPost("create-district")]
        public async Task<IActionResult> CreateDistrict([FromBody] CreateOrUpdateDistrictDTO districtDTO)
        {
            var result = await _districtService.CreateDistrictAsync(districtDTO);
            return Ok(result);
        }
        [HttpPut("update-district/{id}")]
        public async Task<IActionResult> UpdateDistrict(Guid id, [FromBody] CreateOrUpdateDistrictDTO districtDTO)
        {
            var result = await _districtService.UpdateDistrictAsync(districtDTO);
            return Ok(result);
        }
        [HttpDelete("delete-district/{id}")]
        public async Task<IActionResult> DeleteDistrict(Guid id)
        {
            var result = await _districtService.DeleteDistrictAsync(id);
            return Ok(result);
        }
    }
}
