using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public virtual ICollection<ProductDetailCategory> ProductDetailCategories { get; set; }
    }
} 