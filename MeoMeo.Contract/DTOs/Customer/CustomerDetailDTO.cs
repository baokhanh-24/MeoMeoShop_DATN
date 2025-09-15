using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Customer
{
    public class CustomerDetailDTO
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreationTime { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        public ECustomerStatus Status { get; set; }
        public string? Avatar { get; set; }
        public int Gender { get; set; }

        // Thống kê
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CanceledOrders { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    public class CustomerOrdersRequestDTO
    {
        public Guid CustomerId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? CodeFilter { get; set; }
        public DateTime? CreationDateStartFilter { get; set; }
        public DateTime? CreationDateEndFilter { get; set; }
        public EOrderStatus? OrderStatusFilter { get; set; }
    }
}
