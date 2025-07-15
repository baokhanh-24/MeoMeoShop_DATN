using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums;

public enum EHistoryType
{ 
    [Display(Name = "Tạo mới")]
    Create,
    [Display(Name = "Cập nhật")]
    Update,
    [Display(Name = "Xóa")]
    Delete
}