using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }
        [HttpGet("find-order-detail-by-id/{id}")]
        public async Task<IActionResult> GetOrderDetailById(Guid id)
        {
            var result = await _orderDetailService.GetOrderDetailByIdAsync(id);
            return Ok(result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderDetailService.GetAllDetailAsync();
            return Ok(result);
        }

        [HttpPost("create-order-detail")]
        public async Task<IActionResult> CreaterOrderDetail([FromBody] CreateOrUpdateOrderDetailDTO orderDetail)
        {
            var result = await _orderDetailService.CreateOrderDetailAsync(orderDetail);
            return Ok(result);
        }

        [HttpPut("update-order-detail/{id}")]
        public async Task<IActionResult> UpdateOrderDetail(Guid id, [FromBody] CreateOrUpdateOrderDetailDTO orderDetail)
        {
            var result = await _orderDetailService.UpdateOrderDetailAsync(orderDetail);
            return Ok(result);
        }

        [HttpDelete("delete-order-detail/{id}")]
        public async Task<IActionResult> DeleteOrderDetail(Guid id)
        {
            var result = await _orderDetailService.DeleteOrderOrderDetailAsync(id);
            return Ok(result);
        }
    }
}
