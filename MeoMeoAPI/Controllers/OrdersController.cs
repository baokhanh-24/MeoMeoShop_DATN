using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Contract.DTOs.Payment;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Shared.Utilities;
using MeoMeo.Shared.VnPay;
using MeoMeo.Shared.VnPay.Enums;
using MeoMeo.Shared.VnPay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MeoMeo.API.Extensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly PaymentOptions _paymentOptions;
        private readonly IVnpay _vnpay;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(
            IOrderService orderService,
            IOptions<PaymentOptions> paymentOptions,
            IVnpay vnpay,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _vnpay = vnpay;
            _httpContextAccessor = httpContextAccessor;
            _paymentOptions = paymentOptions.Value;
            _vnpay.Initialize(_paymentOptions.Vnpay.TmnCode, _paymentOptions.Vnpay.HashSecret, _paymentOptions.Vnpay.BaseUrl, _paymentOptions.Vnpay.PaymentBackReturnUrl);
        }

        
        [HttpGet("get-list-order-async")]
        public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetAllCustomersAsync([FromQuery] GetListOrderRequestDTO request)
        {
            var result = await _orderService.GetListOrderAsync(request);
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(id);
                if (result == null)
                {
                    return NotFound("Không tìm thấy đơn hàng");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // API lấy danh sách đơn hàng của user hiện tại
        [HttpGet("get-my-orders")]
        public async Task<IActionResult> GetMyOrdersAsync([FromQuery] GetListOrderRequestDTO request)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để xem đơn hàng"
                });
            }

            var result = await _orderService.GetListOrderByCustomerAsync(request, customerId);
            return Ok(result);
        }

        [HttpGet("get-orders-by-customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerIdAsync(Guid customerId, [FromQuery] GetListOrderRequestDTO request)
        {
            try
            {
                var result = await _orderService.GetListOrderByCustomerAsync(request, customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPut("update-status-order-async")]
        public async Task<BaseResponse> UpdateStatusOrderAsync([FromBody] UpdateStatusOrderRequestDTO request)
        {
            var result = await _orderService.UpdateStatusOrderAsync(request);
            return result;
        }

        [HttpGet("history/{orderId}")]
        public async Task<GetListOrderHistoryResponseDTO> GetOrderHistoryAsync(Guid orderId)
        {
            var result = await _orderService.GetListOrderHistoryAsync(orderId);
            return result;
        }

        [HttpDelete("delete-order/{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            return Ok(result);
        }

        // API hủy đơn hàng
        [HttpPut("cancel-order/{orderId}")]
        public async Task<IActionResult> CancelOrderAsync(Guid orderId)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để hủy đơn hàng"
                });
            }

            // Tạo request để hủy đơn hàng
            var cancelRequest = new UpdateStatusOrderRequestDTO
            {
                OrderIds = new List<Guid> { orderId },
                Status = EOrderStatus.Canceled,
                Reason = "Đơn hàng bị hủy bởi khách hàng"
            };

            var result = await _orderService.UpdateStatusOrderAsync(cancelRequest);
            return Ok(result);
        }

        // API hủy đơn hàng với lý do
        [HttpPut("cancel-order-with-reason/{orderId}")]
        public async Task<IActionResult> CancelOrderWithReasonAsync(Guid orderId, [FromBody] CancelOrderWithReasonRequestDTO request)
        {
            try
            {
                // Tạo request để hủy đơn hàng với lý do
                var cancelRequest = new UpdateStatusOrderRequestDTO
                {
                    OrderIds = new List<Guid> { orderId },
                    Status = EOrderStatus.Canceled,
                    Reason = request.Reason
                };

                var result = await _orderService.UpdateStatusOrderAsync(cancelRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi hủy đơn hàng: {ex.Message}"
                });
            }
        }

        [HttpPost("create-pos-order")]
        public async Task<CreatePosOrderResultDTO> CreatePosOrder([FromBody] CreatePosOrderDTO input)
        {
            var employeeId = _httpContextAccessor.HttpContext.GetCurrentEmployeeId();
            return await _orderService.CreatePosOrderAsync(employeeId, input);
        }

        // API lấy danh sách đơn hàng đang xử lý
        [HttpGet("get-pending-orders")]
        public async Task<IActionResult> GetPendingOrdersAsync([FromQuery] GetPendingOrdersRequestDTO request)
        {
            try
            {
                // Get current employee ID from JWT token
                var employeeId = _httpContextAccessor.HttpContext.GetCurrentEmployeeId();
                if (employeeId != Guid.Empty)
                {
                    request.EmployeeId = employeeId;
                }

                var result = await _orderService.GetPendingOrdersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi lấy danh sách đơn hàng đang xử lý: {ex.Message}"
                });
            }
        }

        // API xóa đơn hàng đang xử lý
        [HttpDelete("delete-pending-order/{orderId}")]
        public async Task<IActionResult> DeletePendingOrderAsync(Guid orderId)
        {
            try
            {
                var result = await _orderService.DeletePendingOrderAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi xóa đơn hàng: {ex.Message}"
                });
            }
        }

        // API cập nhật đơn hàng POS
        [HttpPut("update-pos-order/{orderId}")]
        public async Task<IActionResult> UpdatePosOrderAsync(Guid orderId, [FromBody] CreatePosOrderDTO input)
        {
            try
            {
                var employeeId = _httpContextAccessor.HttpContext.GetCurrentEmployeeId();
                var result = await _orderService.UpdatePosOrderAsync(orderId, employeeId, input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new CreatePosOrderResultDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi cập nhật đơn hàng: {ex.Message}"
                });
            }
        }
    }
}
