using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities;

public class OrderReturn : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string Code { get; set; }
    public string Reason { get; set; }
    public EOrderReturnStatus Status { get; set; }

    // Refund method selection
    public ERefundMethod RefundMethod { get; set; }

    // Financial fields
    public decimal? PayBackAmount { get; set; } // Số tiền thực tế đã hoàn trả
    public DateTime? PayBackDate { get; set; } // Ngày hoàn trả tiền

    // Optional bank info for transfer
    public string? BankName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountNumber { get; set; }

    // Optional contact info if receive via ship/store
    public string? ContactPhone { get; set; }
    public string? ContactName { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;
    public DateTime? LastModifiedTime { get; set; }

    public virtual Order Order { get; set; }
    public virtual Customers Customer { get; set; }
    public virtual ICollection<OrderReturnItem> Items { get; set; }
    public virtual ICollection<OrderReturnFile> Files { get; set; }
}