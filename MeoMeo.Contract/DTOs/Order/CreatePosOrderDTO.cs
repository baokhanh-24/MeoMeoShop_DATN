using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order
{
    public class CreatePosOrderDTO
    {
        public EOrderType Type { get; set; } = EOrderType.Store;
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public EOrderPaymentMethod PaymentMethod { get; set; } = EOrderPaymentMethod.Cash;
        public string? Note { get; set; }
        public decimal ShippingFee { get; set; }
        public string? DiscountCode { get; set; }
        public Guid CustomerId { get; set; }
        public CreatePosDeliveryDTO? Delivery { get; set; }

        public List<CreatePosOrderItemDTO> Items { get; set; } = new();
    }

    public class CreatePosOrderItemDTO
    {
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class CreatePosDeliveryDTO
    {
        public string ConsigneeName { get; set; } = string.Empty;
        public string ConsigneePhone { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int CommuneId { get; set; }
        public string ConsigneeAddress { get; set; } = string.Empty;
        public string? ConsigneeNote { get; set; }
    }

    public class CreatePosOrderResultDTO
    {
        public Guid OrderId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public BaseStatus ResponseStatus { get; set; } = BaseStatus.Success;
    }
}


