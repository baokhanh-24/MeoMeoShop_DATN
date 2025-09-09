using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class OrderReturnListDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public EOrderReturnStatus Status { get; set; }
        public ERefundMethod RefundMethod { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }

        // Financial info
        public decimal? PayBackAmount { get; set; }
        public DateTime? PayBackDate { get; set; }

        // Calculated fields
        public decimal TotalRefundAmount { get; set; }
        public int TotalItemCount { get; set; }
        public string StatusDisplayName { get; set; } = string.Empty;
        public string RefundMethodDisplayName { get; set; } = string.Empty;

        // Customer info
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
    }
}
