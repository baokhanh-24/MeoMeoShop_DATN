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
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var result = await _promotionServices.GetPromotionByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-promotion-async")]
        public async Task<IActionResult> CreatePromotion([FromBody] CreateOrUpdatePromotionDTO promotion)
        {
            var result = await _promotionServices.CreatePromotionAsync(promotion);
            return Ok(result);
        }

        [HttpDelete("delete-promotion-async/{id}")]
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            var result = await _promotionServices.DeletePromotionAsync(id);
            return Ok(result);
        }

        [HttpPut("update-promotion-async")]
        public async Task<IActionResult> UpdatePromotion([FromBody] CreateOrUpdatePromotionDTO promotion)
        {
            var result = await _promotionServices.UpdatePromotionAsync(promotion);
            return Ok(result);
        }
    }
}
