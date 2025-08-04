
using System.ComponentModel.DataAnnotations;
using MeoMeo.Domain.Commons.Enums;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class CreateOrUpdateProductDetailDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
       
        public float Price { get; set; }
        public string? Description { get; set; }
      
        public EProductDetailGender Gender { get; set; }
        
        public float StockHeight { get; set; }
       
        public EClosureType ClosureType { get; set; }
       
        public int OutOfStock { get; set; }
      
        public bool AllowReturn { get; set; }
       
        public int Status { get; set; }
       
        public Guid BrandId { get; set; }
      
        public IEnumerable<Guid> SizeIds { get; set; }
       
        public IEnumerable<Guid> ColourIds { get; set; }
      
        public IEnumerable<Guid> SeasonIds { get; set; }
       
        public IEnumerable<Guid> MaterialIds { get; set; }
       
        public IEnumerable<Guid> CategoryIds { get; set; }
      
        public List<ProductMediaUpload>? MediaUploads { get; set; }

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
