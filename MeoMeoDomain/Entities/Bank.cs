namespace MeoMeo.Domain.Entities
{
    public class Bank
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public virtual ICollection<CustomersBank> CustomersBanks { get; set; }
    }
}
