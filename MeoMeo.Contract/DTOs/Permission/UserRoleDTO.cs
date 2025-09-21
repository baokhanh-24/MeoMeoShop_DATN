using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Permission
{
    public class UserRoleDTO : BaseResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
    }

    public class AssignRoleToUserDTO
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class AssignUserToRoleDTO
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class UserWithRolesDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public int Status { get; set; }
        public List<RoleDTO> Roles { get; set; } = new();
    }
}
