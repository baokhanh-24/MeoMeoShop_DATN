using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EOrderType
    {
        [Display(Name = "Tại quyầy")]
        Store,
        [Display(Name = "Online")]
        Online,
        [Display(Name = "Đi giao")]
        Shiiping
    }
}
