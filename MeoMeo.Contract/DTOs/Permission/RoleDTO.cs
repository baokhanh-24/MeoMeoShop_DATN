using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Permission
{
    public class RoleDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Status { get; set; }
        public List<PermissionDTO> Permissions { get; set; } = new();
        public List<UserRoleDTO> UserRoles { get; set; } = new();
    }

    public class CreateOrUpdateRoleDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Status { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class AssignPermissionsToRoleDTO
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
