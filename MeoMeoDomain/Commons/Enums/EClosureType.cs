using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums;

public enum EClosureType
{
    [Display(Name = "Buộc dây")]
    LaceUp,

    [Display(Name = "Quai dán")]
    Velcro,

    [Display(Name = "Không dây")]
    SlipOn,

    [Display(Name = "Khóa kéo")]
    Zipper,

    [Display(Name = "Quai cài")]
    Buckle,

    [Display(Name = "Co giãn")]
    Elastic
}