namespace MeoMeo.Domain.Entities
{
    public class Province
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        // public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual ICollection<District> Districts { get; set; }
    }
}
