using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Permission
{
    public class RoleDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Status { get; set; }
        public List<UserRoleDTO> UserRoles { get; set; } = new();
    }

    public class CreateOrUpdateRoleDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Status { get; set; }
    }
}
