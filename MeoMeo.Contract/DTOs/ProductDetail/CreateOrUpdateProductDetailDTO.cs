
using MeoMeo.Domain.Commons.Enums;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class CreateOrUpdateProductDetailDTO
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public float Price { get; set; }
        public string? Description { get; set; }
        public EProductDetailGender Gender { get; set; }
        public float StockHeight { get; set; }
        public float ShoeLength { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public int Status { get; set; }
        public Guid  BrandId { get; set; }
        public List<Guid>  SizeIds { get; set; }
        public List<Guid>  ColourIds { get; set; }
        public List<Guid>  SeasonIds { get; set; }
        public List<Guid>  MaterialIds { get; set; }
        public List<ProductMediaUpload>  Images { get; set; }
        
    }

    public class ProductMediaUpload
    {
        public Guid? Id { get; set; }
        public IFormFile UploadFile { get; set; }
    }
    
}
