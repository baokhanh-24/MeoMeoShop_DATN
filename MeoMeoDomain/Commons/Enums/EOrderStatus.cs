using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EOrderStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending,
        [Display(Name = "Đã xác nhận")]
        Confirmed,
        [Display(Name = "Đang vận chuyển")]
        InTransit,
        [Display(Name = "Đã hủy")]
        Canceled,
        [Display(Name = "Hoàn thành")]
        Completed,   
        [Display(Name = "Chờ xác nhận hoàn hàng")]
        PendingReturn,   
        [Display(Name = "Đã hoàn hàng")]
        Returned,   
        [Display(Name = "Từ chối cho phép hoàn hàng")]
        RejectReturned,
    }
}
