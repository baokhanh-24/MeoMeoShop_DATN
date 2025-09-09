namespace MeoMeo.Contract.DTOs.Order;

public class CreatePaymentUrlDTO
{
    public Guid OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? ReturnUrl { get; set; }
}