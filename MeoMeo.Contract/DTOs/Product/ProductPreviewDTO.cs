using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs.Product
{
    public class ProductPreviewDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
    }

    public class TopRatedProductDTO
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public float AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
    }

    public class BestSellerItemDTO
    {
        public Guid ProductId { get; set; }
        public Guid ProductDetailId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int TotalSold { get; set; }
        public float? Price { get; set; }
        public float? Discount { get; set; }
        public string SizeValue { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
    }

    public class CategoryHoverResponseDTO
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductPreviewDTO> Products { get; set; } = new List<ProductPreviewDTO>();
    }
}



