using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionDetailController : ControllerBase
    {
        private readonly IPromotionDetailServices _promotionDetailServices;

        public PromotionDetailController(IPromotionDetailServices promotionDetailServices)
        {
            _promotionDetailServices = promotionDetailServices;
        }

        [HttpGet("get-all-promotion-detail-async")]
        public async Task<IActionResult> GetAllPromotionDetailAsync()
        {
            var result = await _promotionDetailServices.GetAllPromotionDetailAsync();
            return Ok(result);
        }

        [HttpGet("find-promotion-detail-by-id-async/{id}")]
        public async Task<IActionResult> GetPromotionDetailById(Guid id)
        {
            var result = await _promotionDetailServices.GetPromotionDetailByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-promotion-detail-async")]
        public async Task<IActionResult> CreatePromotionDetail([FromBody] CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var result = await _promotionDetailServices.CreatePromotionDetailAsync(promotionDetail);
            return Ok(result);
        }

        [HttpDelete("delete-promotion-detail-async/{id}")]
        public async Task<IActionResult> DeletePromotionDetail(Guid id)
        {
            var result = await _promotionDetailServices.DeletePromotionDetailAsync(id);
            return Ok(result);
        }

        [HttpPut("update-promotion-detail-async")]
        public async Task<IActionResult> UpdatePromotionDetail([FromBody] CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var result = await _promotionDetailServices.UpdatePromotionDetailAsync(promotionDetail);
            return Ok(result);
        }
    }
}
