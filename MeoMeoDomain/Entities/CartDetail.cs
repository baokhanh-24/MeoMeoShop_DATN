using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class CartDetail : BaseEntity
    {
        public Guid CartId { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid PromotionDetailId { get; set; }
        public float Discount { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual PromotionDetail PromotionDetails { get; set; }
    }
}
