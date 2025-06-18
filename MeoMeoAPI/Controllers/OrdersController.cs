using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
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

        [HttpGet("find-order-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllAsync();
            return Ok(result);
        }


        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrUpdateOrderDTO order)
        {
            var result = await _orderService.CreateOrderAsync(order);
            return Ok(result);
        }

        [HttpPut("update-order/{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] CreateOrUpdateOrderDTO order)
        {
            var result = await _orderService.UpdateOrderAsync(order);
            //var result = await _orderService.UpdateOrderAsync(order);
            return Ok(result);
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
