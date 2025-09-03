namespace MeoMeo.Contract.DTOs
{
    public class CommuneDTO
    {
        public Guid Id { get; set; }
        public Guid DistrictId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
