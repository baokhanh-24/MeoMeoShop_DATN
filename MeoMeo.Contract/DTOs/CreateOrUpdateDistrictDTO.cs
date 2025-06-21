namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateDistrictDTO
    {
        public Guid Id { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
