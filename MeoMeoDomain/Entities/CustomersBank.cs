using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class CustomersBank : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Beneficiary { get; set; }
        public int Status { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual Bank Bank { get; set; }
    }
}
