using MeoMeo.Domain.Commons;
namespace MeoMeo.Domain.Entities;

public class PermissionGroup : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int? Order { get; set; }

    public virtual PermissionGroup Parent { get; set; }
    public virtual ICollection<Permission> Permissions { get; set; }
}
