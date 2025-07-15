using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionDetailsController : ControllerBase
    {
        private readonly IPromotionDetailServices _promotionDetailServices;

        public PromotionDetailsController(IPromotionDetailServices promotionDetailServices)
        {
            _promotionDetailServices = promotionDetailServices;
        }

        [HttpGet("get-all-promotion-detail-async")]
        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>> GetAllPromotionDetailAsync([FromQuery] GetListPromotionDetailRequestDTO request)
        {
            var result = await _promotionDetailServices.GetAllPromotionDetailAsync(request);
            return result;
        }

        [HttpGet("find-promotion-detail-by-id-async/{id}")]
        public async Task<IActionResult> GetPromotionDetailByIdAsync(Guid id)
        {
            var result = await _promotionDetailServices.GetPromotionDetailByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-promotion-detail-async")]
        public async Task<IActionResult> CreatePromotionDetailAsync([FromBody] CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var result = await _promotionDetailServices.CreatePromotionDetailAsync(promotionDetail);
            return Ok(result);
        }

        [HttpDelete("delete-promotion-detail-async/{id}")]
        public async Task<IActionResult> DeletePromotionDetailAsync(Guid id)
        {
            var result = await _promotionDetailServices.DeletePromotionDetailAsync(id);
            return Ok(result);
        }

        [HttpPut("update-promotion-detail-async/{id}")]
        public async Task<IActionResult> UpdatePromotionDetailAsync(Guid id,[FromBody] CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var result = await _promotionDetailServices.UpdatePromotionDetailAsync(promotionDetail);
            return Ok(result);
        }
    }
}
