using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Payment;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentTransactionController(IPaymentTransactionService paymentTransactionService)
        {
            _paymentTransactionService = paymentTransactionService;
        }

        /// <summary>
        /// Get payment transaction by transaction code
        /// </summary>
        [HttpGet("by-code/{transactionCode}")]
        public async Task<ActionResult<PaymentTransactionDTO>> GetByTransactionCode(string transactionCode)
        {
            try
            {
                var result = await _paymentTransactionService.GetByTransactionCodeAsync(transactionCode);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy giao dịch" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get payment transaction by order ID
        /// </summary>
        [HttpGet("by-order/{orderId}")]
        public async Task<ActionResult<PaymentTransactionDTO>> GetByOrderId(Guid orderId)
        {
            try
            {
                var result = await _paymentTransactionService.GetByOrderIdAsync(orderId);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy giao dịch cho đơn hàng này" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get payment transactions by customer ID
        /// </summary>
        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<List<PaymentTransactionDTO>>> GetByCustomerId(Guid customerId)
        {
            try
            {
                var result = await _paymentTransactionService.GetByCustomerIdAsync(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retry failed transaction
        /// </summary>
        [HttpPost("{transactionId}/retry")]
        public async Task<ActionResult<BaseResponse>> RetryFailedTransaction(Guid transactionId)
        {
            try
            {
                var result = await _paymentTransactionService.RetryFailedTransactionAsync(transactionId);
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

        /// <summary>
        /// Refund transaction
        /// </summary>
        [HttpPost("{transactionId}/refund")]
        public async Task<ActionResult<BaseResponse>> RefundTransaction(
            Guid transactionId,
            [FromBody] RefundTransactionRequestDTO request)
        {
            try
            {
                var result = await _paymentTransactionService.RefundTransactionAsync(
                    transactionId,
                    request.RefundAmount,
                    request.Reason);
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

        /// <summary>
        /// Get payment statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<PaymentStatisticsDTO>> GetPaymentStatistics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var totalRevenue = await _paymentTransactionService.GetTotalRevenueAsync(fromDate, toDate);
                var successfulTransactions = await _paymentTransactionService.GetSuccessfulTransactionCountAsync(fromDate, toDate);
                var paymentMethodStats = await _paymentTransactionService.GetPaymentMethodStatsAsync(fromDate, toDate);

                var statistics = new PaymentStatisticsDTO
                {
                    TotalRevenue = totalRevenue,
                    SuccessfulTransactionCount = successfulTransactions,
                    PaymentMethodStats = paymentMethodStats,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class RefundTransactionRequestDTO
    {
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class PaymentStatisticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int SuccessfulTransactionCount { get; set; }
        public Dictionary<string, int> PaymentMethodStats { get; set; } = new();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
