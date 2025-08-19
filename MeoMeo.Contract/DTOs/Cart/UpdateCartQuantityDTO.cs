namespace MeoMeo.Contract.DTOs;

public class UpdateCartQuantityDTO
{
    public Guid? CustomerId { get; set; }
    public Guid CartDetailId { get; set; }
    public int Quantity { get; set; }
} 