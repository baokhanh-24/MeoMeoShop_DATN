using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class ResetPasswordHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public DateTime ExpriedDate { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? SuccessDate { get; set; }
        public EResetPasswordHistoryStatus Status { get; set; }
        public virtual User User { get; set; }
    }
}
