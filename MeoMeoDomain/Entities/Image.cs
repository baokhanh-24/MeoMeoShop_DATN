using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Image : BaseEntity
    {
        public Guid ProductDetailId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string URL { get; set; }
        public int Status { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
