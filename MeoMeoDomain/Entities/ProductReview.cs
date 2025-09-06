using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;

public class ProductReview:BaseEntityAudited
{
    public string Content { get; set; }
    public string? Answer { get; set; }
    public DateTime? ReplyDate { get; set; }
    public float Rating { get; set; }
    public bool IsHidden { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductDetailId { get; set; }
    public virtual Customers Customer { get; set; }
    public virtual Order Order { get; set; }
    public virtual ProductDetail ProductDetail { get; set; }
    public virtual ICollection<ProductReviewFile> ProductReviewFiles { get; set; }
}