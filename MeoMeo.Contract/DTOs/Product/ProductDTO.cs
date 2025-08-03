using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Product
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        
        // Related collections
        public List<Guid> SizeIds { get; set; } = new List<Guid>();
        public List<string> SizeValues { get; set; } = new List<string>();
        public List<Guid> ColourIds { get; set; } = new List<Guid>();
        public List<string> ColourNames { get; set; } = new List<string>();
        public List<Guid> MaterialIds { get; set; } = new List<Guid>();
        public List<string> MaterialNames { get; set; } = new List<string>();
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<Guid> SeasonIds { get; set; } = new List<Guid>();
        public List<string> SeasonNames { get; set; } = new List<string>();
        
        // Product variants summary
        public int VariantCount { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public int TotalStock { get; set; }
        public int TotalSellNumber { get; set; }
        public int TotalViewNumber { get; set; }
        
        // Media
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
