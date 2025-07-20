using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        public float Discount { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EVoucherType Type { get; set; }
        public decimal MinOrder { get; set; }
        public float MaxDiscount { get; set; }
        public int? MaxTotalUse { get; set; }
        public int? MaxTotalUsePerCustomer { get; set; }
        public virtual ICollection<Domain.Entities.Order> Orders { get; set; }
        
        public EVoucherStatus Status
        {
            get
            {
                var now = DateTime.Now;
                if (now < StartDate) return EVoucherStatus.Upcoming;
                if (now > EndDate) return EVoucherStatus.Expired;
                return EVoucherStatus.Active;
            }
        }
    }
}
