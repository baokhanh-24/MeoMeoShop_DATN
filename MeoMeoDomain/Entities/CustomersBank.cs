using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class CustomersBank : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Beneficiary { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; }

    }
}
