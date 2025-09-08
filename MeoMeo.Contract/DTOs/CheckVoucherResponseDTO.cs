using MeoMeo.Contract.Commons;
using System;

namespace MeoMeo.Contract.DTOs
{
    public class CheckVoucherResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public float Discount { get; set; }
        public decimal MinOrder { get; set; }
        public float MaxDiscount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
