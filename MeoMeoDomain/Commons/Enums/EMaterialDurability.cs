using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EMaterialDurability
    {
        [Display(Name = "Thấp")]
        Low,

        [Display(Name = "Trung bình")]
        Medium,

        [Display(Name = "Cao")]
        High
    }
}
