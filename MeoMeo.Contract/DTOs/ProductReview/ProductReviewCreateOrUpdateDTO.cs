using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs.ProductReview
{
    public class ProductReviewCreateOrUpdateDTO
    {
        public Guid? Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public bool IsHidden { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public List<ProductReviewFileUpload>? MediaUploads { get; set; }
    }

    public class ProductReviewFileUpload
    {
        public Guid? Id { get; set; }
        
        public IFormFile? UploadFile { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }
}