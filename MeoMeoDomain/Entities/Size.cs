using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Size : BaseEntity
    {
        public string Value { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public virtual ICollection<ProductDetailSize> ProductDetailSizes { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<InventoryBatch> InventoryBatches { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<PromotionDetail> PromotionDetails { get; set; }
    }
}
