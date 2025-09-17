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
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly PaymentOptions _paymentOptions;
        private readonly IVnpay _vnpay;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(
            IOrderService orderService,
            IPaymentTransactionService paymentTransactionService,
            IOptions<PaymentOptions> paymentOptions,
            IVnpay vnpay,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _paymentTransactionService = paymentTransactionService;
            _vnpay = vnpay;
            _httpContextAccessor = httpContextAccessor;
            _paymentOptions = paymentOptions.Value;
            _vnpay.Initialize(_paymentOptions.Vnpay.TmnCode, _paymentOptions.Vnpay.HashSecret, _paymentOptions.Vnpay.BaseUrl, _paymentOptions.Vnpay.PaymentBackReturnUrl);
        }

        [HttpPost("take-vn-pay")]
        public async Task<string> CreatePaymentUrlAsync([FromBody] CreatePaymentUrlDTO input)
        {
            var ipAddress = NetworkHelper.GetIpAddress(_httpContextAccessor.HttpContext);

            // Create payment transaction record first
            var createTransactionRequest = new CreatePaymentTransactionDTO
            {
                OrderId = input.OrderId,
                CustomerId = input.CustomerId,
                Amount = input.Amount,
                Description = string.IsNullOrWhiteSpace(input.Description) ? $"Thanh toan don hang {input.OrderId}" : input.Description,
                IpAddress = ipAddress,
                PaymentMethod = "VNPAY"
            };

            var transactionResult = await _paymentTransactionService.CreatePaymentTransactionAsync(createTransactionRequest);
            if (transactionResult.ResponseStatus != BaseStatus.Success)
            {
                throw new Exception(transactionResult.Message);
            }

            var transactionCode = transactionResult.TransactionCode;

            var request = new PaymentRequest
            {
                PaymentId = DateTime.Now.Ticks,
                Money = input.Amount,
                Description = createTransactionRequest.Description,
                IpAddress = ipAddress,
                BankCode = BankCode.ANY,
                CreatedDate = DateTime.Now,
                Currency = Currency.VND,
                Language = DisplayLanguage.Vietnamese
            };

            // Use transaction code as VnpTxnRef for tracking
            var paymentUrl = _vnpay.GetPaymentUrl(request);

            // Replace the auto-generated TxnRef with our transaction code
            paymentUrl = paymentUrl.Replace($"vnp_TxnRef={request.PaymentId}", $"vnp_TxnRef={transactionCode}");

            return paymentUrl;
        }

        [HttpGet("call-back-vn-pay")]
        public async Task<object> GetCallBackAsync()
        {
            try
            {
                var query = _httpContextAccessor.HttpContext.Request.Query;

                // Extract VNPay callback data
                var callback = new VnpayCallbackDTO
                {
                    vnp_TxnRef = query["vnp_TxnRef"].ToString(),
                    vnp_TransactionNo = query["vnp_TransactionNo"].ToString(),
                    vnp_Amount = query["vnp_Amount"].ToString(),
                    vnp_BankCode = query["vnp_BankCode"].ToString(),
                    vnp_BankTranNo = query["vnp_BankTranNo"].ToString(),
                    vnp_CardType = query["vnp_CardType"].ToString(),
                    vnp_PayDate = query["vnp_PayDate"].ToString(),
                    vnp_OrderInfo = query["vnp_OrderInfo"].ToString(),
                    vnp_ResponseCode = query["vnp_ResponseCode"].ToString(),
                    vnp_TransactionStatus = query["vnp_TransactionStatus"].ToString(),
                    vnp_SecureHash = query["vnp_SecureHash"].ToString()
                };

                // Build raw data string for logging
                var rawData = string.Join("&", query.Select(kvp => $"{kvp.Key}={kvp.Value}"));

                // Process callback through PaymentTransactionService
                var result = await _paymentTransactionService.ProcessVnpayCallbackAsync(callback, rawData);

                if (result.ResponseStatus == BaseStatus.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        transactionRef = callback.vnp_TxnRef
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = result.Message,
                    transactionRef = callback.vnp_TxnRef
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
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
