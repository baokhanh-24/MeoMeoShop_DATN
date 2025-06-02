using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Customers : BaseEnitityAudited
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string IName { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        public int Status { get; set; }
    }
}
