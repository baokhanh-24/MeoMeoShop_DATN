using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class CreatePartialOrderReturnDTO
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public ERefundMethod RefundMethod { get; set; }

        // Bank info for transfer
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }

        // Contact info for ship/store pickup
        public string? ContactPhone { get; set; }
        public string? ContactName { get; set; }

        [Required]
        public List<OrderReturnItemDTO> Items { get; set; } = new();

        // Optional files (images, receipts, etc.)
        public List<OrderReturnFileDTO> Files { get; set; } = new();
    }

    public class OrderReturnItemDTO
    {
        [Required]
        public Guid OrderDetailId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }
    }

    public class OrderReturnFileDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
