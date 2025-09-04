using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Order.Return;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderReturnsController : ControllerBase
    {
        private readonly IOrderReturnService _orderReturnService;

        public OrderReturnsController(IOrderReturnService orderReturnService)
        {
            _orderReturnService = orderReturnService;
        }

        [HttpPost]
        public async Task<CreateOrderReturnResponseDTO> Create([FromBody] CreateOrderReturnRequestDTO request)
        {
            // TODO: replace with actual current user id from auth
            var currentCustomerId = Guid.Empty;
            return await _orderReturnService.CreateAsync(request, currentCustomerId);
        }

        [HttpPut("status")]
        public async Task<UpdateOrderReturnStatusResponseDTO> UpdateStatus([FromBody] UpdateOrderReturnStatusRequestDTO request)
        {
            return await _orderReturnService.UpdateStatusAsync(request);
        }

        [HttpGet("{id}")]
        public async Task<OrderReturnViewDTO?> GetById(Guid id)
        {
            return await _orderReturnService.GetByIdAsync(id);
        }

        [HttpGet("order/{orderId}")]
        public async Task<List<OrderReturnViewDTO>> GetByOrderId(Guid orderId)
        {
            return await _orderReturnService.GetByOrderIdAsync(orderId);
        }
    }
}


