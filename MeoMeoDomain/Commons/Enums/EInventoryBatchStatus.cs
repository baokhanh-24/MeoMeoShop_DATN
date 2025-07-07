using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EInventoryBatchStatus
    {
        [Display(Name = "Nháp")]
        Draft,
        [Display(Name = "Chờ duyệt")]
        PendingApproval,
        [Display(Name = "Đã phê duyệt")]
        Aprroved
    }
}
