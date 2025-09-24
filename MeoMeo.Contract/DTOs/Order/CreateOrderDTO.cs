using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order
{
    public class CreateOrderDTO
    {
        public Guid? VoucherId { get; set; }
        public Guid? DeliveryAddressId { get; set; }
        public EOrderPaymentMethod PaymentMethod { get; set; } = EOrderPaymentMethod.Cash;
        public string? Note { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public List<Guid> CartItems { get; set; } = new List<Guid>();
    }

}
