using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order.Return;

public class OrderReturnViewDTO
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public EOrderReturnStatus Status { get; set; }
    public ERefundMethod RefundMethod { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactName { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModifiedTime { get; set; }
    public List<OrderReturnItemViewDTO> Items { get; set; } = new();
    public List<OrderReturnFileViewDTO> Files { get; set; } = new();
}

public class OrderReturnItemViewDTO
{
    public Guid Id { get; set; }
    public Guid OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}

public class OrderReturnFileViewDTO
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}


