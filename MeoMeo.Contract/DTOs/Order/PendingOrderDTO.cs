using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order
{
    public class PendingOrderDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public EOrderStatus Status { get; set; }
        public EOrderType Type { get; set; }
        public EOrderPaymentMethod PaymentMethod { get; set; }
        public string? Note { get; set; }
        public int ItemCount { get; set; }
        public bool IsDraft { get; set; }
    }
}
