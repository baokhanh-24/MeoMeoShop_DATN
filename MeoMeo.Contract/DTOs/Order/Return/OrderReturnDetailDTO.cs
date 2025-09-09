using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class OrderReturnDetailDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public EOrderReturnStatus Status { get; set; }
        public ERefundMethod RefundMethod { get; set; }

        // Financial info
        public decimal? PayBackAmount { get; set; }
        public DateTime? PayBackDate { get; set; }

        // Bank info
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }

        // Contact info
        public string? ContactPhone { get; set; }
        public string? ContactName { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }

        // Related data
        public OrderReturnOrderInfo Order { get; set; } = new();
        public List<OrderReturnItemDetailDTO> Items { get; set; } = new();
        public List<OrderReturnFileDetailDTO> Files { get; set; } = new();

        // Calculated fields
        public decimal TotalRefundAmount { get; set; }
        public int TotalItemCount { get; set; }
        public string StatusDisplayName { get; set; } = string.Empty;
        public string RefundMethodDisplayName { get; set; } = string.Empty;
    }

    public class OrderReturnOrderInfo
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public EOrderStatus Status { get; set; }
    }

    public class OrderReturnItemDetailDTO
    {
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public string? Reason { get; set; }

        // Product info from OrderDetail
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public float UnitPrice { get; set; }
        public float OriginalPrice { get; set; }
        public float Discount { get; set; }

        // Calculated
        public decimal RefundAmount { get; set; }
        public int AvailableQuantity { get; set; } // Max quantity available for return
    }

    public class OrderReturnFileDetailDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
