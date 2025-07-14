using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EMaterialWeight
    {
        [Display(Name = "Nhẹ")]
        Light = 0,

        [Display(Name = "Trung bình")]
        Medium = 1,

        [Display(Name = "Nặng")]
        Heavy = 2
    }
}
