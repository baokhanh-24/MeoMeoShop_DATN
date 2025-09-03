namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateDeliveryAddressDTO
    {
        public Guid? Id { get; set; }
        public Guid? CustomerId { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? CommuneId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}