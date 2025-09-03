using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Image : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string URL { get; set; }
        public virtual Product Product { get; set; }
    }
}