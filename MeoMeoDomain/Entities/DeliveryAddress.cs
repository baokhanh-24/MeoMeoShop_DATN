using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class DeliveryAddress : BaseEnitityAudited
    {
        public Guid CustomerId { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string Name { get; set; }
        public string PhoneNumbe { get; set; }
        public string Address { get; set; }


    }
}
