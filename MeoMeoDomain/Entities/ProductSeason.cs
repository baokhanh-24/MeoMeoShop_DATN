namespace MeoMeo.Domain.Entities
{
    public class ProductSeason
    {
        public Guid SeasonId { get; set; }
        public Guid ProductId { get; set; }
        public virtual Season Season { get; set; }
        public virtual Product Product { get; set; }
    }
}
