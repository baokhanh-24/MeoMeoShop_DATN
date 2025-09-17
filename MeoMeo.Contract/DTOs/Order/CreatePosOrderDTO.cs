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
        public EOrderStatus Status { get; set; }
        public List<CreatePosOrderItemDTO> Items { get; set; } = new();
    }

    public class CreatePosOrderItemDTO
    {
        public Guid Id { get; set; } // OrderDetail Id (cho update)
        public Guid ProductDetailId { get; set; }
        public Guid? PromotionDetailId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public float Price { get; set; } // Final price after discount
        public float OriginalPrice { get; set; } // Original price before discount
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public float Discount { get; set; } // Discount amount
        public string Image { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
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

        // Thông tin đầy đủ để in hóa đơn
        public DateTime CreationTime { get; set; }
        public EOrderType Type { get; set; }
        public EOrderStatus Status { get; set; }
        public EOrderPaymentMethod PaymentMethod { get; set; }
        public float TotalPrice { get; set; }
        public float ShippingFee { get; set; }
        public string? Note { get; set; }

        // Thông tin khách hàng
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        // Thông tin nhân viên
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeePhoneNumber { get; set; } = string.Empty;

        // Thông tin giao hàng
        public CreatePosDeliveryDTO? Delivery { get; set; }

        // Chi tiết sản phẩm
        public List<MeoMeo.Contract.DTOs.OrderDetail.OrderDetailDTO> OrderDetails { get; set; } = new List<MeoMeo.Contract.DTOs.OrderDetail.OrderDetailDTO>();
    }
}


