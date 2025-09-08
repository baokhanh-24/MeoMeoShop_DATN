using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Wishlist
    {
        public Guid CustomerId { get; set; }
        public Guid ProductDetailId { get; set; }

        public virtual Customers Customers { get; set; }
        public virtual ProductDetail ProductDetails { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}


