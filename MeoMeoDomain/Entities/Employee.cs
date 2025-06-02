using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBird { get; set; }
        public string Address { get; set; }
    }
}
