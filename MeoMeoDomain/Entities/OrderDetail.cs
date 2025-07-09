using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid? PromotionDetailId { get; set; }
        public Guid? InventoryBatchId { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public float? Discount { get; set; }
        public string Image { get; set; }
        public virtual Order Order { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual PromotionDetail PromotionDetail { get; set; }
        public virtual InventoryBatch? InventoryBatch { get; set; }
    }
}
