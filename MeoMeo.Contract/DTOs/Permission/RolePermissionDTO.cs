using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Permission
{
    public class RolePermissionDTO
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }

    public class SubPermissionGroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public List<PermissionDTO> Permissions { get; set; } = new();
    }
}
