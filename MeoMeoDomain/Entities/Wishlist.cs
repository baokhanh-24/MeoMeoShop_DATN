using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Wishlist : BaseEntityAudited
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}


