using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Voucher : BaseEnitityAudited
    {
        public float Discount { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Type { get; set; }
        public Decimal MinOrder { get; set; }
        public float MaxDiscount { get; set; }
        public int? MaxTotalUse { get; set; }
        public int? MaxTotalUsePerCustomer { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
