using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.PromotionDetail
{
    public class PromotionDetailWithProductInfoDTO
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public Guid ProductDetailId { get; set; }
        public float Discount { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }

        // Product Detail Information
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public float OriginalPrice { get; set; }
        public float DiscountPrice { get; set; }

        // Size and Colour Information
        public string SizeName { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
    }
}
