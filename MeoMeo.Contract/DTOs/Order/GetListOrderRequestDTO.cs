using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order;

public class GetListOrderRequestDTO:BasePaging
{
    public string? CodeFilter { get; set; }
    public string? CustomerNameFilter { get; set; }
    public string? CustomerPhoneNumberFilter { get; set; }
    public string? CustomerEmailFilter { get; set; }
    public DateTime? CreationDateStartFilter { get; set; }
    public DateTime? CreationDateEndFilter { get; set; }  
    public DateTime? ShippingDateStartFilter { get; set; }
    public DateTime?ShippingDateEndFilter { get; set; }
    public EOrderPaymentMethod? PaymentMethodFilter { get; set; }
    public EOrderStatus? OrderStatusFilter { get; set; }
}