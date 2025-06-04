using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Colour : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public virtual ICollection<ProductDetailColour> ProductDetailColours { get; set; }
    }
}
