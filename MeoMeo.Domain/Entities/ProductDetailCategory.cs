namespace MeoMeo.Domain.Entities
{
    public class ProductDetailCategory
    {
        public Guid ProductDetailId { get; set; }
        public Guid CategoryId { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual Category Category { get; set; }
    }
} 