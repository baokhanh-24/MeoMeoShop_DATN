using System;
using System.Collections.Generic;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.ProductReview
{
    public class ProductReviewDTO:BaseResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Answer { get; set; }
        public DateTime? ReplyDate { get; set; }
        public decimal Rating { get; set; }
        public bool IsHidden { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public DateTime CreationTime { get; set; }
        public List<ProductReviewFileDTO> ProductReviewFiles { get; set; } = new();

        // Customer information
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAvatar { get; set; } = string.Empty;
        
    }
}