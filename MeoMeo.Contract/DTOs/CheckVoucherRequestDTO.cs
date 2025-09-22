using System;

namespace MeoMeo.Contract.DTOs
{
    public class CheckVoucherRequestDTO
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
        public Guid? CustomerId { get; set; } // Optional để check MaxTotalUsePerCustomer
    }
}
