using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;
public class Role:BaseEntity
{
    public int? Status { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }
    
}
