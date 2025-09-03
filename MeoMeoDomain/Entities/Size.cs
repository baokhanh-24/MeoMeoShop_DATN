using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Size : BaseEntity
    {
        public string Value { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
