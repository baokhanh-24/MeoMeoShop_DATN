using MeoMeo.Domain.Commons.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class CreatePartialOrderReturnDTO
    {
        public Guid OrderId { get; set; }

        public string Reason { get; set; } = string.Empty;

        public ERefundMethod RefundMethod { get; set; }

        // Bank info for transfer
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }

        // Contact info for ship/store pickup
        public string? ContactPhone { get; set; }
        public string? ContactName { get; set; }

        public List<OrderReturnItemDTO> Items { get; set; } = new();

        public List<OrderReturnFileUpload> FileUploads { get; set; } = new();
    }

    public class OrderReturnItemDTO
    {
        public Guid OrderDetailId { get; set; }
        public int Quantity { get; set; }

        public string? Reason { get; set; }
    }

    public class OrderReturnFileUpload
    {
        public Guid? Id { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }
}
