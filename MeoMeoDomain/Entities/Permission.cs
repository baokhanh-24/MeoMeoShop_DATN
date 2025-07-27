using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;
public class Permission :BaseEntity
{
    public string Function { get; set; }
    public Guid PermissionGroupId { get; set; }
    public string Command { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public  virtual PermissionGroup PermissionGroup { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }

}
