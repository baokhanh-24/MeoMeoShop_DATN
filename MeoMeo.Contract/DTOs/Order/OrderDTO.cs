using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.OrderDetail;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order;
public class OrderDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? VoucherId { get; set; }
    public Guid? DeliveryAddressId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string EmployeePhoneNumber { get; set; } = string.Empty;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string? EmployeeEmail { get; set; }
    public string? CustomerEmail { get; set; }
    public Decimal TotalPrice { get; set; }
    public Decimal? DiscountPrice { get; set; }
    public Decimal? ShippingFee { get; set; }
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
    public IEnumerable<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();

    // Order Return Information
    public OrderReturnSummaryDTO? OrderReturn { get; set; }
}

// Summary DTO for Order Return information
public class OrderReturnSummaryDTO
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public EOrderReturnStatus Status { get; set; }
    public ERefundMethod RefundMethod { get; set; }
    public decimal? PayBackAmount { get; set; }
    public DateTime? PayBackDate { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModifiedTime { get; set; }
    public int TotalItemCount { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public List<OrderReturnFileSummaryDTO> Files { get; set; } = new();
    public List<OrderReturnItemSummaryDTO> Items { get; set; } = new();
    public OrderReturnBankInfoDTO? BankInfo { get; set; }

    // Contact info for ViaShipper and InStore methods
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }

    public string StatusDisplayName { get; set; } = string.Empty;
    public string RefundMethodDisplayName { get; set; } = string.Empty;
}

public class OrderReturnItemSummaryDTO
{
    public Guid Id { get; set; }
    public Guid OrderDetailId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SizeName { get; set; } = string.Empty;
    public string ColourName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ImageUrl { get; set; }
}

public class OrderReturnBankInfoDTO
{
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
}

public class OrderReturnFileSummaryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
