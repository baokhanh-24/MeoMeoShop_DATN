using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.ProductReview;

public class GetListProductReviewDTO : BasePaging
{
    public string ListProductDetailIds { get; set; }
    public int? Rating { get; set; }
}