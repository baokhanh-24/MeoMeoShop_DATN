using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionServices _promotionServices;

        public PromotionsController(IPromotionServices promotionServices)
        {
            _promotionServices = promotionServices;
        }

        [HttpGet("get-all-promotion-async")]
        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO>> GetAllPromotionAsync([FromQuery] GetListPromotionRequestDTO request)
        {
            var result = await _promotionServices.GetAllPromotionAsync(request);
            return result;
        }

        [HttpGet("find-promotion-by-id-async/{id}")]
        public async Task<IActionResult> GetPromotionByIdAsync(Guid id)
        {
            var result = await _promotionServices.GetPromotionByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("get-promotion-detail-async/{id}")]
        public async Task<IActionResult> GetPromotionDetailAsync(Guid id)
        {
            var result = await _promotionServices.GetPromotionDetailAsync(id);
            return Ok(result);
        }

        [HttpPost("create-promotion-async")]
        public async Task<IActionResult> CreatePromotionAsync([FromBody] CreateOrUpdatePromotionDTO promotion)
        {
            var result = await _promotionServices.CreatePromotionAsync(promotion);
            return Ok(result);
        }

        [HttpPost("create-promotion-with-details-async")]
        public async Task<IActionResult> CreatePromotionWithDetailsAsync([FromBody] UpdatePromotionWithDetailsDTO request)
        {
            var result = await _promotionServices.CreatePromotionWithDetailsAsync(request);
            return Ok(result);
        }

        [HttpDelete("delete-promotion-async/{id}")]
        public async Task<IActionResult> DeletePromotionAsync(Guid id)
        {
            var result = await _promotionServices.DeletePromotionAsync(id);
            return Ok(result);
        }

        [HttpPut("update-promotion-async/{id}")]
        public async Task<IActionResult> UpdatePromotionAsync(Guid id, [FromBody] CreateOrUpdatePromotionDTO promotion)
        {
            var result = await _promotionServices.UpdatePromotionAsync(promotion);
            return Ok(result);
        }

        [HttpPut("update-promotion-with-details-async")]
        public async Task<IActionResult> UpdatePromotionWithDetailsAsync([FromBody] UpdatePromotionWithDetailsDTO request)
        {
            var result = await _promotionServices.UpdatePromotionWithDetailsAsync(request);
            return Ok(result);
        }
    }
}
