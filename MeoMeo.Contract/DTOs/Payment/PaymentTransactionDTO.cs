using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs.Payment
{
    public class PaymentTransactionDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string VnpTransactionNo { get; set; } = string.Empty;
        public string VnpTxnRef { get; set; } = string.Empty;
        public string VnpOrderInfo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string VnpBankCode { get; set; } = string.Empty;
        public string VnpBankTranNo { get; set; } = string.Empty;
        public string VnpCardType { get; set; } = string.Empty;
        public string VnpPayDate { get; set; } = string.Empty;
        public EPaymentStatus Status { get; set; }
        public string VnpResponseCode { get; set; } = string.Empty;
        public string VnpTransactionStatus { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Currency { get; set; } = "VND";
        public bool IsValidated { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }

        // Display fields
        public string StatusDisplayName { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }

    public class CreatePaymentTransactionDTO
    {
        public Guid OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "VNPAY";
    }

    public class VnpayCallbackDTO
    {
        public string vnp_TmnCode { get; set; } = string.Empty;
        public string vnp_Amount { get; set; } = string.Empty;
        public string vnp_BankCode { get; set; } = string.Empty;
        public string vnp_BankTranNo { get; set; } = string.Empty;
        public string vnp_CardType { get; set; } = string.Empty;
        public string vnp_PayDate { get; set; } = string.Empty;
        public string vnp_OrderInfo { get; set; } = string.Empty;
        public string vnp_TransactionNo { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_TransactionStatus { get; set; } = string.Empty;
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string vnp_SecureHashType { get; set; } = string.Empty;
        public string vnp_SecureHash { get; set; } = string.Empty;
    }
}
