using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs;

    public class CreateOrUpdatePermissionGroupDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
    }

