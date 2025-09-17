using System;

namespace MeoMeo.Contract.DTOs
{
    public class GetAvailableVouchersRequestDTO
    {
        public Guid CustomerId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
