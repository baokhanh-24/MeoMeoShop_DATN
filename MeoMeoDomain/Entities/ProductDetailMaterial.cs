namespace MeoMeo.Domain.Entities
{
    public class ProductDetailMaterial
    {
        public Guid ProductDetailId { get; set; }
        public Guid MaterialId { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual Material Material { get; set; }
    }
}
