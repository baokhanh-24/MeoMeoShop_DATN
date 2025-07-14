using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EOrderPaymentMethod
    {
        [Display(Name = "Tiền mặt")]
        Cash,
        [Display(Name = "Chuyển khoản")]
        Transfer
    }
}
