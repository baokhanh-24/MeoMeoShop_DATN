using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("get-list-order-async")]
        public async Task<PagingExtensions.PagedResult<OrderDTO>> GetAllCustomersAsync([FromQuery] GetListOrderRequestDTO request)
        {
            var result = await _orderService.GetListOrderAsync(request);
            return result;
        }   
        [HttpPut("update-status-order-async")]
        public async Task<BaseResponse> UpdateStatusOrderAsync([FromBody] UpdateStatusOrderRequestDTO request)
        {
            var result = await _orderService.UpdateStatusOrderAsync(request);
            return result;
        }
        // DELETE api/<OrdersController>/5
        [HttpDelete("delete-order/{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            return Ok(result);
        }
    }
}
