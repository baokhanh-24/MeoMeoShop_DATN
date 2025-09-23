namespace MeoMeo.Domain.Entities
{
    public class PromotionDetail
    {
        public Guid PromotionId { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid Id { get; set; }
        public float Discount { get; set; }
        public string? Note { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
