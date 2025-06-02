namespace MeoMeo.Domain.Entities
{
    public class Commune
    {
        public Guid Id { get; set; }
        public Guid DistrictId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
