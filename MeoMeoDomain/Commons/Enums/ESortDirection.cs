using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums;

public enum ESortDirection
{
    [Display(Name = "Tăng dần")]
    Asc,
    [Display(Name = "Giảm dần")]
    Desc
}