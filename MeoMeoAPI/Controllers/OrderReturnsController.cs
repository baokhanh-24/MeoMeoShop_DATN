using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Attributes;
using MeoMeo.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderReturnController : ControllerBase
    {
        private readonly IOrderReturnService _orderReturnService;

        public OrderReturnController(IOrderReturnService orderReturnService)
        {
            _orderReturnService = orderReturnService;
        }

        [HttpPost("create-partial-return")]
        public async Task<ActionResult<BaseResponse>> CreatePartialOrderReturn([FromBody] CreatePartialOrderReturnDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
                var result = await _orderReturnService.CreatePartialOrderReturnAsync(customerId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                });
            }
        }

        [HttpGet("my-returns")]
        public async Task<ActionResult<PagingExtensions.PagedResult<OrderReturnListDTO>>> GetMyOrderReturns([FromQuery] GetOrderReturnRequestDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
                var result = await _orderReturnService.GetMyOrderReturnsAsync(customerId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<PagingExtensions.PagedResult<OrderReturnListDTO>>> GetAllOrderReturns([FromQuery] GetOrderReturnRequestDTO request)
        {
            try
            {
                var result = await _orderReturnService.GetOrderReturnsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReturnDetailDTO>> GetOrderReturnById(Guid id)
        {
            try
            {
                var result = await _orderReturnService.GetOrderReturnByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy yêu cầu hoàn trả" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<BaseResponse>> UpdateOrderReturnStatus(Guid id, [FromBody] UpdateOrderReturnStatusRequestDTO request)
        {
            try
            {
                var result = await _orderReturnService.UpdateOrderReturnStatusAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                });
            }
        }

        [HttpPut("{id}/payback")]
        public async Task<ActionResult<BaseResponse>> UpdatePayBackAmount(Guid id, [FromBody] UpdatePayBackAmountDTO request)
        {
            try
            {
                var result = await _orderReturnService.UpdatePayBackAmountAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<BaseResponse>> CancelOrderReturn(Guid id)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
                var result = await _orderReturnService.CancelOrderReturnAsync(customerId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                });
            }
        }

        [HttpGet("order/{orderId}/available-items")]
        public async Task<ActionResult<List<OrderReturnItemDetailDTO>>> GetAvailableItemsForReturn(Guid orderId)
        {
            try
            {
                var result = await _orderReturnService.GetAvailableItemsForReturnAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("order/{orderId}/can-return")]
        public async Task<ActionResult<bool>> CanOrderBeReturned(Guid orderId)
        {
            try
            {
                var result = await _orderReturnService.CanOrderBeReturnedAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
