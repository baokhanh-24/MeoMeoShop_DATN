using System;

namespace MeoMeo.Contract.DTOs
{
    public class CheckVoucherRequestDTO
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }
}
