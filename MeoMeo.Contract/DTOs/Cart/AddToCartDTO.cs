namespace MeoMeo.Contract.DTOs;

public class AddToCartDTO
{
    public Guid? CustomerId { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid ProductDetailId { get; set; }
    public int Quantity { get; set; }
}