using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Shared.IServices
{
    public interface IProductReviewClientService
    {
        Task<IEnumerable<ProductReviewDTO>> GetAllAsync();
    }

    public class ProductReviewDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public float Rating { get; set; }
        public bool IsHidden { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public DateTime CreationTime { get; set; }
        public List<ProductReviewFileDTO> ProductReviewFiles { get; set; } = new();
    }

    public class ProductReviewFileDTO
    {
        public Guid Id { get; set; }
        public Guid ProductReviewId { get; set; }
        public string FileUrl { get; set; }
        public int FileType { get; set; }
    }
}


