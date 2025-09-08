namespace MeoMeo.Contract.DTOs.Wishlist
{
    public class CreateOrUpdateWishlistDTO
    {
        public Guid? CustomerId { get; set; }
        public Guid ProductDetailId { get; set; }
    }
}
