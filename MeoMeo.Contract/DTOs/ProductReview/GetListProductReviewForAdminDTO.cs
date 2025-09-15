using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.ProductReview
{
    public class GetListProductReviewForAdminDTO : BasePaging
    {
        public string? SearchTerm { get; set; }
        public bool? IsHidden { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxRating { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool? HasReply { get; set; }
    }
}
