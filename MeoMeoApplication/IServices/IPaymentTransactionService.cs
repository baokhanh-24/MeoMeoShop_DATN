using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Payment;
using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IPaymentTransactionService
    {
        Task<CreatePaymentTransactionResponseDTO> CreatePaymentTransactionAsync(CreatePaymentTransactionDTO request);
        Task<BaseResponse> ProcessVnpayCallbackAsync(VnpayCallbackDTO callback, string rawData);
        Task<PaymentTransactionDTO?> GetByTransactionCodeAsync(string transactionCode);
        Task<PaymentTransactionDTO?> GetByOrderIdAsync(Guid orderId);
        Task<List<PaymentTransactionDTO>> GetByCustomerIdAsync(Guid customerId);
        Task<BaseResponse> RetryFailedTransactionAsync(Guid transactionId);
        Task<BaseResponse> RefundTransactionAsync(Guid transactionId, decimal refundAmount, string reason);

        // Statistics
        Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<int> GetSuccessfulTransactionCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, int>> GetPaymentMethodStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
