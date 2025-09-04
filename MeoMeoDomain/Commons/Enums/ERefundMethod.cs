using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum ERefundMethod
    {
        [Display(Name = "Chuyển khoản")] BankTransfer = 0,
        [Display(Name = "Nhận qua ship")] ViaShipper = 1,
        [Display(Name = "Nhận tại cửa hàng")] InStore = 2
    }
}


