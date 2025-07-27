using MeoMeo.Contract.DTOs.Permission;

namespace MeoMeo.Contract.DTOs.PermissionGroup;

public class PermissionGroupDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public bool? IsGranted { get; set; } = false;
    public int TotalCount { get; set; } 
    public int? Order { get; set; }
    public List<SubPermissionGroupDTO> SubPermissionGroups { get; set; }
 
}

public class SubPermissionGroupDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public int? Order { get; set; }
    public bool? IsGranted { get; set; } = false;
    public int TotalCount { get; set; } 
    public IEnumerable<PermissionDTO> Permissions { get; set; }
}
