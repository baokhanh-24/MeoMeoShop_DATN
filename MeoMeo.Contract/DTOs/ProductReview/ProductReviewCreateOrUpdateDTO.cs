using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs.ProductReview
{
    public class ProductReviewCreateOrUpdateDTO
    {
        public Guid ?Id { get; set; }
        public string Content { get; set; }
        public float Rating { get; set; }
        public bool IsHidden { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public List<ProductReviewFileUpload> Files { get; set; }
    }
    public class ProductReviewFileUpload
    {
        public Guid? Id { get; set; }
        public IFormFile UploadFile { get; set; }
    }
} 