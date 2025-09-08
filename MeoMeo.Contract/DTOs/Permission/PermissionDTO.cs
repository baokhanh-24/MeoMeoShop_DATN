using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Permission
{
    public class PermissionDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Function { get; set; } = string.Empty;
        public Guid PermissionGroupId { get; set; }
        public string PermissionGroupName { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
        public bool? IsGranted { get; set; } = false;
    }

    public class CreateOrUpdatePermissionDTO
    {
        public Guid? Id { get; set; }
        public string Function { get; set; } = string.Empty;
        public Guid PermissionGroupId { get; set; }
        public string Command { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}