using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs
{
    public class AvailableVoucherDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public float Discount { get; set; }
        public decimal MinOrder { get; set; }
        public float MaxDiscount { get; set; }
        public EVoucherType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxTotalUse { get; set; }
        public int? MaxTotalUsePerCustomer { get; set; }
        public decimal CalculatedDiscountAmount { get; set; } // Số tiền giảm được tính toán
        public bool CanUse { get; set; } // Có thể sử dụng không
        public string? Reason { get; set; } // Lý do không thể sử dụng
    }
}
