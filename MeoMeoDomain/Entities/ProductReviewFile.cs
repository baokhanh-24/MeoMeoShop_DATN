using MeoMeo.Domain.Commons;
using System;

namespace MeoMeo.Domain.Entities
{
    public class ProductReviewFile : BaseEntity
    {
        public Guid ProductReviewId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public int FileType { get; set; } 

        public virtual ProductReview ProductReview { get; set; }
    }
} 