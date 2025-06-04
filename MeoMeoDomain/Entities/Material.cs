using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class Material : BaseEntity
    {
        public string Name { get; set; }
        public EMaterialDurability Durability { get; set; }
        public bool WaterProof { get; set; }
        public EMaterialWeight Weight { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public virtual ICollection<ProductDetailMaterial> ProductDetailMaterials { get; set; }
    }
}
