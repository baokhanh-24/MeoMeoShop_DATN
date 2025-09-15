using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.PromotionDetail
{
    public class GetPromotionDetailWithProductInfoRequestDTO : BasePaging
    {
        public Guid PromotionId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
