using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order
{
    public class CreateOrderDTO
    {
        public Guid? Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? DeliveryAddressId { get; set; }
        public EOrderPaymentMethod PaymentMethod { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? ExpectReceiveDate { get; set; }
        public EOrderType Type { get; set; }
        public string? Note { get; set; }
        public string? Reason { get; set; }
        public List<Guid> CartItems { get; set; } = new List<Guid>();
    }

}
