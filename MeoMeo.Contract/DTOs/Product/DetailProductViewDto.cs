using MeoMeo.Contract.DTOs.ProductDetail;

namespace MeoMeo.Contract.DTOs.Product
{
    public class DetailProductViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Status { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? OutOfStockQuantity { get; set; }
        public int? NumberExpires { get; set; }
        public bool AllowReturn { get; set; }
        public float? Length { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string SeoTitle { get; set; } = string.Empty;
        public string SeoKeyword { get; set; } = string.Empty;
        public string SeoDescription { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }

    public class FileViewDto
    {
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsVideo => Type?.StartsWith("video") == true;
    }

    public class ProductHistoryDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }

    public class DropdownListDto
    {
        public List<DropdownItemDto> ListType { get; set; } = new();
        public List<DropdownItemDto> ListSupp { get; set; } = new();
        public List<DropdownItemDto> ListCate { get; set; } = new();
        public List<DropdownItemDto> ListUnit { get; set; } = new();
        public List<DropdownItemDto> ListBrand { get; set; } = new();
    }

    public class DropdownItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Disabled { get; set; }
    }
}