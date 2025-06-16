using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionServices _promotionServices;

        public PromotionController(IPromotionServices promotionServices)
        {
            _promotionServices = promotionServices;
        }

        [HttpGet("get-all-promotion-async")]
        public async Task<IActionResult> GetAllPromotionAsync()
        {
            var result = await _promotionServices.GetAllPromotionAsync();
            return Ok(result);
        }

        [HttpGet("find-promotion-by-id-async/{id}")]
        public async Task<IActionResult> GetPromotionByIdAsync(Guid id)
        {
            var result = await _promotionServices.GetPromotionByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-promotion-async")]
        public async Task<IActionResult> CreatePromotionAsync([FromBody] CreateOrUpdatePromotionDTO promotion)
        {
            var result = await _promotionServices.CreatePromotionAsync(promotion);
            return Ok(result);
        }

        [HttpDelete("delete-promotion-async/{id}")]
        public async Task<IActionResult> DeletePromotionAsync(Guid id)
        {
            var result = await _promotionServices.DeletePromotionAsync(id);
            return Ok(result);
        }

        [HttpPut("update-promotion-async/{id}")]
        public async Task<IActionResult> UpdatePromotionAsync(Guid id,[FromBody] CreateOrUpdatePromotionDTO promotion)
        {
            var result = await _promotionServices.UpdatePromotionAsync(promotion);
            return Ok(result);
        }
    }
}
