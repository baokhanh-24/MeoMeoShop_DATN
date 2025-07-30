using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class ProductDetailDetailDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Thumbnail { get; set; }
        public string? Barcode { get; set; }
        public string? Sku { get; set; }
        public float Price { get; set; }
        public float? Discount { get; set; }
        public Guid? PromotionDetailId { get; set; }
        public string? Description { get; set; }
        public EProductDetailGender Gender { get; set; }
        public float StockHeight { get; set; }
        public EClosureType ClosureType { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public int Status { get; set; }
        public int? InventoryQuantity { get; set; }
        public int? ViewNumber { get; set; }
        public int? SellNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        
        // Related data for binding
        public Guid BrandId { get; set; }
        public string? BrandName { get; set; }
        public List<Guid> SizeIds { get; set; } = new List<Guid>();
        public List<string> SizeValues { get; set; } = new List<string>();
        public List<Guid> ColourIds { get; set; } = new List<Guid>();
        public List<string> ColourNames { get; set; } = new List<string>();
        public List<Guid> SeasonIds { get; set; } = new List<Guid>();
        public List<string> SeasonNames { get; set; } = new List<string>();
        public List<Guid> MaterialIds { get; set; } = new List<Guid>();
        public List<string> MaterialNames { get; set; } = new List<string>();
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<ProductDetailImageDTO> Images { get; set; } = new List<ProductDetailImageDTO>();
    }

    public class ProductDetailImageDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public bool IsVideo => ContentType?.StartsWith("video/") == true;
    }
} 