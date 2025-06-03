using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Size : BaseEntity
    {
        public string Value { get; set; }
        public string Code { get; set; }
        public virtual ICollection<ProductDetailSize> ProductDetailSizes { get; set; }
    }
}
