using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Material : BaseEntity
    {
        public string Name { get; set; }
        public int Durability { get; set; }
        public bool WaterProof { get; set; }
        public int Weight { get; set; }
        public string Description { get; set; }
    }
}
