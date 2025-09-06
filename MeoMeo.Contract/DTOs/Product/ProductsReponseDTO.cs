using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Product
{
    public class ProductResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public float? MaxDiscount { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModificationTime { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        // Related collections
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
        
        // Product variants
        public List<ProductDetailGrid> ProductVariants { get; set; } = new List<ProductDetailGrid>();
        
        // Media
        public List<ProductMediaUpload> Media { get; set; } = new List<ProductMediaUpload>();
        
        // Additional fields for product detail page
        public string? Description { get; set; }
        public decimal Rating { get; set; }
        public int SaleNumber { get; set; }
        
        // Rating breakdown
        public int Rating1 { get; set; }
        public int Rating2 { get; set; }
        public int Rating3 { get; set; }
        public int Rating4 { get; set; }
        public int Rating5 { get; set; }
    }
    
    
}
