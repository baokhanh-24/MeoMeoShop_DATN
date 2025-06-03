using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Season : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ProductSeason> ProductSeasons { get; set; }
    }
}
