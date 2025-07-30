using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class ProductDetail : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public EProductDetailGender Gender { get; set; }
        public float StockHeight { get; set; }
        public int OutOfStock { get; set; }
        public EClosureType  ClosureType { get; set; }
        public int? SellNumber { get; set; }
        public int? ViewNumber { get; set; }
        public bool AllowReturn { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int Status { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<InventoryBatch> InventoryBatches { get; set; }
        public virtual ICollection<ProductDetailColour> ProductDetailColours { get; set; }
        public virtual ICollection<ProductDetailSize> ProductDetailSizes { get; set; }
        public virtual ICollection<ProductDetailMaterial> ProductDetailMaterials { get; set; }
        
        public virtual ICollection<PromotionDetail> PromotionDetails { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
