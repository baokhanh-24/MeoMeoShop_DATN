using System;

namespace MeoMeo.Contract.DTOs.ProductReview
{
    public class ProductReviewFileDTO
    {
        public Guid Id { get; set; }
        public Guid ProductReviewId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int FileType { get; set; }
        public long Size { get; set; }
    }
}