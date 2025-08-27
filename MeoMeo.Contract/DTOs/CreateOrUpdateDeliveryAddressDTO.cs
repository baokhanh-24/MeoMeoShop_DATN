namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateDeliveryAddressDTO
    {
        public Guid? Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? CommuneId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
