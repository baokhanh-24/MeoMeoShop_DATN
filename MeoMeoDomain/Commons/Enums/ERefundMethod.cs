using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum ERefundMethod
    {
        [Display(Name = "Chuyển khoản")] BankTransfer = 0,
        [Display(Name = "Nhận tại cửa hàng")] InStore = 1
    }
}


