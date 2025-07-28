
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
        [Required]
        public float Price { get; set; }
        public string? Description { get; set; }
        [Required]
        public EProductDetailGender Gender { get; set; }
        [Required]
        public float StockHeight { get; set; }
        [Required]
        public float ShoeLength { get; set; }
        [Required]
        public int OutOfStock { get; set; }
        [Required]
        public bool AllowReturn { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public Guid BrandId { get; set; }
        [Required]
        public List<Guid> SizeIds { get; set; }
        [Required]
        public List<Guid> ColourIds { get; set; }
        [Required]
        public List<Guid> SeasonIds { get; set; }
        [Required]
        public List<Guid> MaterialIds { get; set; }
        [Required]
        public List<Guid> CategoryIds { get; set; }
        [Required]
        public List<ProductMediaUpload> Images { get; set; }

    }

    public class ProductMediaUpload
    {
        public Guid? Id { get; set; }
        public IFormFile UploadFile { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? Base64Data { get; set; } = string.Empty; // Để lưu base64 ở frontend
        public string? FileName { get; set; } = string.Empty; // Tên file
        public string? ContentType { get; set; } = string.Empty; // Content type
    }

}
