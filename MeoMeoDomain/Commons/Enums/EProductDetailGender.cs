using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EProductDetailGender
    {
        [Display(Name = "Nam")]
        Men,
        [Display(Name = "Nữ")]
        Women,
        [Display(Name = "Unisex")]
        Unisex
    }
}
