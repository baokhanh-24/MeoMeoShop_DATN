using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EPromotionStatus
    {
        [Display(Name = "Chưa diễn ra")]
        NotHappenedYet,
        [Display(Name = "Đang diễn ra")]
        IsGoingOn,
        [Display(Name = "Đã kết thúc")]
        Ended
    }
}
