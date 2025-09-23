using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Attributes;
using MeoMeo.Shared.Constants;
using MeoMeo.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderReturnController : ControllerBase
    {
        private readonly IOrderReturnService _orderReturnService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderReturnController(IOrderReturnService orderReturnService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _orderReturnService = orderReturnService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("create-partial-return")]
        [RequestSizeLimit(200 * 1024 * 1024)]
        public async Task<ActionResult<BaseResponse>> CreatePartialOrderReturn([FromForm] CreatePartialOrderReturnDTO request)
        {
            try
            {
                var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
                if (customerId == Guid.Empty)
                {
                    return BadRequest();
                }
                // Handle file uploads
                var filesToUpload = request.FileUploads?.Where(f => f.UploadFile != null).Select(f => f.UploadFile!).ToList() ?? new List<IFormFile>();
                List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();

                if (filesToUpload.Any())
                {
                    var returnId = Guid.NewGuid();
                    uploadedFiles = await FileUploadHelper.UploadFilesAsync(
                        _environment,
                        filesToUpload,
                        "OrderReturns",
                        returnId,
                        acceptedExtensions: new List<string>
                        {
                            "jpg", "jpeg", "png", "gif", "bmp", "tiff", "webp", "svg", "heic", "heif",
                            "mp4", "avi", "mov", "wmv"
                        },
                        maxFileSizeInBytes: 200 * 1024 * 1024
                    );
                }

                var result = await _orderReturnService.CreatePartialOrderReturnAsync(customerId, request, uploadedFiles);

                if (result.ResponseStatus == BaseStatus.Error)
                {
                    // Rollback uploaded files if service failed
                    FileUploadHelper.DeleteUploadedFiles(_environment, uploadedFiles);
                }

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

        [HttpPut("update-status")]
        public async Task<ActionResult<BaseResponse>> UpdateOrderReturnStatus([FromBody] UpdateOrderReturnStatusRequestDTO request)
        {
            try
            {
                var result = await _orderReturnService.UpdateOrderReturnStatusAsync(request.OrderReturnId, request);
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

        [HttpGet("order/{orderId}/return-info")]
        public async Task<ActionResult<object>> GetOrderReturnInfo(Guid orderId)
        {
            try
            {
                var result = await _orderReturnService.GetOrderReturnInfoAsync(orderId);
                return Ok(new
                {
                    canReturn = result.CanReturn,
                    message = result.Message,
                    returnableProducts = result.ReturnableProducts,
                    nonReturnableProducts = result.NonReturnableProducts
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
