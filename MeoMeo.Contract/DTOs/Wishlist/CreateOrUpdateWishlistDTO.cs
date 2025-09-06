namespace MeoMeo.Contract.DTOs.Wishlist
{
    public class CreateOrUpdateWishlistDTO
    {
        public Guid? Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
    }
}
