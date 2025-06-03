namespace MeoMeo.Domain.Entities
{
    public class ProductDetailSize
    {
        public Guid ProductDetailId { get; set; }
        public Guid SizeId { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual Size Size { get; set; }
    }
}
