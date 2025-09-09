using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EPaymentStatus
    {
        [Display(Name = "Chờ thanh toán")]
        Pending = 0,

        [Display(Name = "Đang xử lý")]
        Processing = 1,

        [Display(Name = "Thành công")]
        Success = 2,

        [Display(Name = "Thất bại")]
        Failed = 3,

        [Display(Name = "Đã hủy")]
        Cancelled = 4,

        [Display(Name = "Hoàn tiền")]
        Refunded = 5,

        [Display(Name = "Hoàn tiền một phần")]
        PartiallyRefunded = 6,

        [Display(Name = "Hết hạn")]
        Expired = 7,

        [Display(Name = "Lỗi hệ thống")]
        SystemError = 8
    }
}
