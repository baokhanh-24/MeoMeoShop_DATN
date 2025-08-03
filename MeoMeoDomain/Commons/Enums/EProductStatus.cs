using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums;

public enum EProductStatus
{ 
    [Display(Name = "Lưu tạm")]
    Draft,
    [Display(Name = "Chờ duyệt")]
    Pending,
    [Display(Name = "Bị từ chối")]
    Rejected,
    [Display(Name = "Ngừng bán")]
    StopSelling,
    [Display(Name = "Đang bán")]
    Selling
}