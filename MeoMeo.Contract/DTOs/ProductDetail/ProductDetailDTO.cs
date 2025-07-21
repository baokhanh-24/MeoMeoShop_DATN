using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class ProductDetailDTO
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
        public float ShoeLength { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public int Status { get; set; }
        public string Images { get; set; }
        public string Sizes { get; set; }
        public string Colours { get; set; }
        public string Materials { get; set; }
        public int? InventoryQuantity { get; set; }
        public int? ViewNumber { get; set; }
        public int? SellNumber { get; set; }
    }
}
