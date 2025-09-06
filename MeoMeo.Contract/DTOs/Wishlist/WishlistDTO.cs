namespace MeoMeo.Contract.DTOs.Wishlist
{
    public class WishlistDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime CreationTime { get; set; }
        
        // Navigation properties
        public string ProductName { get; set; } = string.Empty;
        public string ProductThumbnail { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public bool IsAvailable { get; set; }
    }
}
