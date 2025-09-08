using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Permission
{
    public class PermissionGroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int Order { get; set; }
        public bool IsGranted { get; set; }
        public List<PermissionGroupDTO> Children { get; set; } = new();
        public List<PermissionDTO> Permissions { get; set; } = new();
        public List<SubPermissionGroupDTO> SubPermissionGroups { get; set; } = new();
    }

    public class CreateOrUpdatePermissionGroupDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
    }
}
