namespace MeoMeo.Contract.DTOs
{
    public class DeliveryAddressDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int CommuneId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Navigation properties
        public string ProvinceName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string CommuneName { get; set; } = string.Empty;
    }
}
