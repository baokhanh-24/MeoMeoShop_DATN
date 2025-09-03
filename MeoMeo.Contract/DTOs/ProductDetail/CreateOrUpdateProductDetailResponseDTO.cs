using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class CreateOrUpdateProductDetailResponseDTO:BaseResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public float StockHeight { get; set; }
        public int OutOfStock { get; set; }  // Thay thế OutOfStock
        public EClosureType ClosureType { get; set; }
        public int? SellNumber { get; set; }
        public int? ViewNumber { get; set; }
        public bool AllowReturn { get; set; }
        public int Status { get; set; }
        
        // Thêm Size và Colour
        public Guid SizeId { get; set; }
        public string SizeValue { get; set; }
        public Guid ColourId { get; set; }
        public string ColourName { get; set; }
    }
}
