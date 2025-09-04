using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class DeliveryAddress : BaseEntityAudited
    {
        public Guid CustomerId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int CommuneId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public virtual Customers Customers { get; set; }
        // public virtual Province Province { get; set; }
        // public virtual District District { get; set; }
        // public virtual Commune Commune { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

    }
}
