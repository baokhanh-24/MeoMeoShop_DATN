namespace MeoMeo.Domain.Entities
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime EstablishYear { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public virtual ICollection<Product> Products { get; set; }

    }
}
