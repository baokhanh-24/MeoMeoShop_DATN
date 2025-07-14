using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Cart : BaseEntityAudited
    {
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual Customers Customers { get; set; }
    }
}
