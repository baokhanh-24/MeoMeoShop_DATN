using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; }
        public string Avatar { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public string Email { get; set; }
    }
}
