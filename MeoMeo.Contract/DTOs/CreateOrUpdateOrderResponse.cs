using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateOrderResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? DeliveryAddressId { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerName { get; set; }
        public string EmployeePhoneNumber { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerCode { get; set; }
        public string? EmployeeEmail { get; set; }
        public string? CustomerEmail { get; set; }
        public Decimal TotalPrice { get; set; }
        public EOrderPaymentMethod PaymentMethod { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? ExpectReceiveDate { get; set; }
        public EOrderType Type { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; }
        public string? Note { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? Reason { get; set; }
        public EOrderStatus Status { get; set; }
    }
}
