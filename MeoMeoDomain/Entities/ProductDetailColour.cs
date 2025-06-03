namespace MeoMeo.Domain.Entities
{
    public class ProductDetailColour
    {
        public Guid ProductDetailId { get; set; }
        public Guid ColourId { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual Colour Colour { get; set; }
    }
}
