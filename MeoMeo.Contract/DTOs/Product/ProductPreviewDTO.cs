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

    public class BestSellerItemDTO
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int TotalSold { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
    }

    public class CategoryHoverResponseDTO
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductPreviewDTO> Products { get; set; } = new List<ProductPreviewDTO>();
    }
}



