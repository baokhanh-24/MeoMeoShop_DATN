using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class ResetPasswordHistory : BaseEntity
    {
        public DateTime ExpriedDate { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? SuccessDate { get; set; }
    }
}
