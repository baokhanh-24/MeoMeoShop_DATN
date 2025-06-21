using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateVoucherResponseDTO : BaseResponse
    {
        public Guid? Id { get; set; }
        public float Discount { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EVoucherType Type { get; set; }
        public Decimal MinOrder { get; set; }
        public float MaxDiscount { get; set; }
        public int? MaxTotalUse { get; set; }
        public int? MaxTotalUsePerCustomer { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModificationTime { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
