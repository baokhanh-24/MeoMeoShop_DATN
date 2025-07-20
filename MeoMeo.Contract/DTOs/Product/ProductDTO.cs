namespace MeoMeo.Contract.DTOs.Product
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string? Name { get; set; }
        public string? Thumbnail { get; set; }
    }
}
