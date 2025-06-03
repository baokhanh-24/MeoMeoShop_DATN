using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Product : BaseEnitityAudited
    {
        public Guid BrandId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual ICollection<ProductSeason> ProductSeasons { get; set; }
    }
}
