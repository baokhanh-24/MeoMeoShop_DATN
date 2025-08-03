
using System.ComponentModel.DataAnnotations;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons.Enums;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class CreateOrUpdateProductDetailDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public Guid BrandId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public float StockHeight { get; set; }
        
        public int OutOfStock { get; set; }  
        public EClosureType ClosureType { get; set; }
        public bool AllowReturn { get; set; }
        public int Status { get; set; }
        
        // Thêm Size và Colour
        public Guid SizeId { get; set; }
        public Guid ColourId { get; set; }
        
        // Related data for binding
        public List<Guid> SizeIds { get; set; } = new List<Guid>();
        public List<Guid> ColourIds { get; set; } = new List<Guid>();
        public List<Guid> SeasonIds { get; set; } = new List<Guid>();
        public List<Guid> MaterialIds { get; set; } = new List<Guid>();
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<ProductMediaUpload>? MediaUploads { get; set; } = new List<ProductMediaUpload>();
    }

    

}
