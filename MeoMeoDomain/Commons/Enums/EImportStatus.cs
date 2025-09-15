using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EImportStatus
    {
        [Display(Name = "Lưu tạm")]
        Draft,
        [Display(Name = "Chờ duyệt")]
        PendingApproval,
        [Display(Name = "Đã phê duyệt")]
        Approved,
        [Display(Name = "Bị từ chối")]
        Rejected
    }
}
