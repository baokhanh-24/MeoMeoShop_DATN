using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EOrderReturnStatus
    {
        [Display(Name = "Chờ duyệt hoàn hàng")] Pending = 0,
        [Display(Name = "Đã duyệt hoàn hàng")] Approved = 1,
        [Display(Name = "Từ chối hoàn hàng")] Rejected = 2,
        [Display(Name = "Đã nhận hàng hoàn")] Received = 3,
        [Display(Name = "Hoàn tiền xong")] Refunded = 4
    }
}


