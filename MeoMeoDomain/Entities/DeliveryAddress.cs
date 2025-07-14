using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class DeliveryAddress : BaseEntityAudited
    {
        public Guid CustomerId { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual Province Province { get; set; }
        public virtual District District { get; set; }
        public virtual Commune Commune { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

    }
}
