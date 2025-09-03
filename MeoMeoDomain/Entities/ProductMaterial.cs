using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class ProductMaterial 
    {
        public Guid ProductId { get; set; }
        public Guid MaterialId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Material Material { get; set; }
    }
}
