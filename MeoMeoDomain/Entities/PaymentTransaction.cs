using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        // Basic Info
        public Guid OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public string TransactionCode { get; set; } = string.Empty; // Mã giao dịch hệ thống

        // VNPay Info
        public string VnpTransactionNo { get; set; } = string.Empty; // Mã giao dịch VNPay
        public string VnpTxnRef { get; set; } = string.Empty; // Mã tham chiếu
        public string VnpOrderInfo { get; set; } = string.Empty; // Thông tin đơn hàng
        public decimal Amount { get; set; } // Số tiền
        public string VnpBankCode { get; set; } = string.Empty; // Mã ngân hàng
        public string VnpBankTranNo { get; set; } = string.Empty; // Mã giao dịch ngân hàng
        public string VnpCardType { get; set; } = string.Empty; // Loại thẻ
        public string VnpPayDate { get; set; } = string.Empty; // Ngày thanh toán VNPay format

        // Status & Response
        public EPaymentStatus Status { get; set; } = EPaymentStatus.Pending;
        public string VnpResponseCode { get; set; } = string.Empty; // Mã phản hồi từ VNPay
        public string VnpTransactionStatus { get; set; } = string.Empty; // Trạng thái giao dịch VNPay
        public string ResponseMessage { get; set; } = string.Empty; // Thông điệp phản hồi

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ProcessedAt { get; set; } // Thời điểm xử lý callback
        public DateTime? CompletedAt { get; set; } // Thời điểm hoàn tất

        // Security & Validation
        public string VnpSecureHash { get; set; } = string.Empty; // Hash bảo mật
        public bool IsValidated { get; set; } = false; // Đã validate hash chưa
        public string IpAddress { get; set; } = string.Empty; // IP thực hiện giao dịch

        // Additional Info
        public string PaymentMethod { get; set; } = string.Empty; // ATM/VISA/MASTER/etc
        public string Currency { get; set; } = "VND";
        public string Locale { get; set; } = "vn";

        // Raw Data (for debugging)
        public string? RawRequestData { get; set; } // Dữ liệu request gốc
        public string? RawResponseData { get; set; } // Dữ liệu response gốc

        // Error Handling
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; } = 0;

        // Relations
        public virtual Order Order { get; set; } = null!;
        public virtual Customers? Customer { get; set; }
    }
}
