using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order;
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
        public OrdersController(IOrderService orderService, IOptions<PaymentOptions> paymentOptions, IVnpay vnpay, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _vnpay = vnpay;
            _httpContextAccessor = httpContextAccessor;
            _paymentOptions = paymentOptions.Value;
            _vnpay.Initialize(_paymentOptions.Vnpay.TmnCode, _paymentOptions.Vnpay.HashSecret, _paymentOptions.Vnpay.BaseUrl, _paymentOptions.Vnpay.PaymentBackReturnUrl);
        }
        
        [HttpPost("take-vn-pay")]
        public async Task<string> CreatePaymentUrlAsync([FromBody] CreatePaymentUrlDTO input)
        {
            var ipAddress = NetworkHelper.GetIpAddress(_httpContextAccessor.HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

            var request = new PaymentRequest
            {
                PaymentId = DateTime.Now.Ticks,
                Money = input.Amount,
                Description = string.IsNullOrWhiteSpace(input.Description) ? $"Thanh toan don hang {input.OrderId}" : input.Description,
                IpAddress = ipAddress,
                BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
            };

            var paymentUrl = _vnpay.GetPaymentUrl(request);
            return await Task.FromResult(paymentUrl);
        }

        [HttpGet("call-back-vn-pay")]
        public async Task<object> GetCallBackAsync()
        {
            
            try
            {
                var paymentResult = _vnpay.GetPaymentResult(_httpContextAccessor.HttpContext.Request.Query);

                if (paymentResult.IsSuccess)
                {
                    return await Task.FromResult(paymentResult);
                }

                return await Task.FromResult("");
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ex.Message);
            }
            
            
        }
        
        [HttpGet("get-list-order-async")]
        public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetAllCustomersAsync([FromQuery] GetListOrderRequestDTO request)
        {
            var result = await _orderService.GetListOrderAsync(request);
            return result;
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

        [HttpPost("create-pos-order")]
        public async Task<CreatePosOrderResultDTO> CreatePosOrder([FromBody] CreatePosOrderDTO input)
        {
            return await _orderService.CreatePosOrderAsync(input);
        }
    }
}
