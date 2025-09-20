using MeoMeo.Contract.Commons;
using System;

namespace MeoMeo.Contract.DTOs.Product
{
    public class ProductSearchRequestDTO : BasePaging
    {
        public string? SearchKeyword { get; set; }
        public string? SKUFilter { get; set; }
        public string? NameFilter { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStockOnly { get; set; } = true;
        public Guid? SizeId { get; set; }
        public Guid? ColourId { get; set; }
    }

    public class ProductSearchResponseDTO : BaseResponse
    {
        public Guid ProductDetailId { get; set; }
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SizeValue { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public int OutOfStock { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int SaleNumber { get; set; }
        public bool IsActive { get; set; }
        public bool AllowReturn { get; set; }
        public decimal? MaxDiscount { get; set; }
        public int Weight { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Shoe-specific fields
        public float StockHeight { get; set; } // Chiều cao đế
        public int Length { get; set; } // Chiều dài giày
        public int Width { get; set; } // Chiều rộng giày  
        public int Height { get; set; } // Chiều cao giày
        public string ClosureType { get; set; } = string.Empty; // Loại khóa (dây buộc, khóa kéo, etc.)

        // Rating breakdown
        public int Rating1 { get; set; }
        public int Rating2 { get; set; }
        public int Rating3 { get; set; }
        public int Rating4 { get; set; }
        public int Rating5 { get; set; }
    }
}
