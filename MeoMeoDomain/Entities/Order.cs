using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Order : BaseEntity
    {
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
        public int PaymentMethod { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? ExpectReceiveDate { get; set; }
        public int Type { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; }
        public string? Note { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? Reason { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Voucher? Voucher { get; set; }
        public virtual DeliveryAddress? Address { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
