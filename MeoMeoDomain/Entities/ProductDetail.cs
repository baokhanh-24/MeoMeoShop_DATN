using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class ProductDetail : BaseEntity
    {
        public Guid ProductId { get; set; }
        
        // Thêm 2 field này để tạo variant
        public Guid SizeId { get; set; }
        public Guid ColourId { get; set; }
        
        // Thông tin variant
        public string Sku { get; set; }  // VD: "NIKE-AM270-BLK-40"
        public float Price { get; set; }
        public int OutOfStock { get; set; }  // Thay thế OutOfStock
        
        // Thông tin chung
        public float StockHeight { get; set; }
        public EClosureType ClosureType { get; set; }
        public int? SellNumber { get; set; }
        public int? ViewNumber { get; set; }
        public bool AllowReturn { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public EProductStatus Status { get; set; }
        
        // Thông tin vận chuyển - cần thiết cho GHN
        public int Weight { get; set; } = 500; // Trọng lượng (gram) - mặc định 500g
        public int Length { get; set; } = 15;  // Chiều dài (cm) - mặc định 15cm
        public int Width { get; set; } = 15;   // Chiều rộng (cm) - mặc định 15cm
        public int Height { get; set; } = 15;  // Chiều cao (cm) - mặc định 15cm
        public int? MaxBuyPerOrder { get; set; }// Số lượng được mua tối đa trên 1 đơn hàng
        
        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
        public virtual Colour Colour { get; set; }
        
        // Bỏ các collection không cần thiết
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<InventoryBatch> InventoryBatches { get; set; }
        public virtual ICollection<PromotionDetail> PromotionDetails { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        
    }
}
