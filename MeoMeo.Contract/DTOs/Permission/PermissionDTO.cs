namespace MeoMeo.Contract.DTOs.Permission;

public class PermissionDTO
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string Function { get; set; }
    public string Command { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; }
    public int? Order { get; set; }
    public bool? IsGranted { get; set; } = false;
}