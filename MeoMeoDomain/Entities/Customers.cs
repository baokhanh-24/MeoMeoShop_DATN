using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class Customers : BaseEntityAudited
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        
        public int Gender { get; set; }
        public ECustomerStatus Status { get; set; }
        public virtual User User { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<CustomersBank> CustomersBanks { get; set; }
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
    }
}
