using MeoMeo.Domain.Commons.Enums;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Contract.DTOs.Product
{
    public class CreateOrUpdateProductDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        
        public string? Description { get; set; }
        public Guid BrandId { get; set; }
        
        public List<Guid> SeasonIds { get; set; } = new List<Guid>();
       
        public List<Guid> MaterialIds { get; set; } = new List<Guid>();

        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<ProductDetailGrid> ProductVariants { get; set; }
        public List<ProductMediaUpload>? MediaUploads { get; set; }

    }

    public class ProductDetailGrid
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string? Sku { get; set; }
        public Guid SizeId { get; set; }
        public string? SizeName { get; set; }
        public Guid ColourId { get; set; }
        public string? ColourName { get; set; }
        public float Price { get; set; }
        public float? Discount { get; set; }
        public int OutOfStock { get; set; }  
        public float StockHeight { get; set; }
        public EClosureType ClosureType { get; set; }
        public int? SellNumber { get; set; }
        public int? ViewNumber { get; set; }
        public bool AllowReturn { get; set; }
        public EProductStatus Status { get; set; }
        public int InventoryQuantity { get; set; }
        
        // Thông tin vận chuyển - cần thiết cho GHN
        public int Weight { get; set; } = 500; // Trọng lượng (gram)
        public int Length { get; set; } = 15;  // Chiều dài (cm)
        public int Width { get; set; } = 15;   // Chiều rộng (cm)
        public int Height { get; set; } = 15;  // Chiều cao (cm)
        
        // Giới hạn mua hàng
        public int? MaxBuyPerOrder { get; set; } // Số lượng được mua tối đa trên 1 đơn hàng
    }
    public class ProductMediaUpload
    {
        public Guid? Id { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? Base64Data { get; set; } = string.Empty; // Để lưu base64 ở frontend
        public string? FileName { get; set; } = string.Empty; // Tên file
        public string? ContentType { get; set; } = string.Empty; // Content type
    }


}
