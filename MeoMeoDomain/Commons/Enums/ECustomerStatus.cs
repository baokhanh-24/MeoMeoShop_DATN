using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum ECustomerStatus
    {
        [Display(Name = "Ngừng hoạt động")]
        Disabled,
        [Display(Name = "Đang hoạt động")]
        Enabled,
        [Display(Name = "Bị khóa")]
        Locked
    }
}
