using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class ProductDetailDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Barcode { get; set; }
        public string? Sku { get; set; }
        public float Price { get; set; }
        public string? Description { get; set; }
        public EProductDetailGender Gender { get; set; }
        public float StockHeight { get; set; }
        public float ShoeLength { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public int Status { get; set; }
    }
}
