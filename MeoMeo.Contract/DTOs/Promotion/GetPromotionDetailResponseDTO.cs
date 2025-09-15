using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.PromotionDetail;

namespace MeoMeo.Contract.DTOs.Promotion
{
    public class GetPromotionDetailResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Danh sách sản phẩm trong promotion
        public List<PromotionDetailWithProductInfoDTO> Products { get; set; } = new();

        // Thống kê
        public int TotalProducts { get; set; }
        public float TotalDiscountAmount { get; set; }
        public float AverageDiscountPercent { get; set; }
    }
}
