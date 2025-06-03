namespace MeoMeo.Domain.Entities
{
    public class District
    {
        public Guid Id { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public virtual Province Province { get; set; }
        public virtual ICollection<Commune> Communes { get; set; }
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
    }
}
