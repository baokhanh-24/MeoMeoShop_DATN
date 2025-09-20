using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class User:BaseEntityAudited
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Avatar { get; set; }
        public DateTime LastLogin { get; set; }
        public string Email { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedEndDate { get; set; }
        public int Status { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
    }
}
