namespace MeoMeo.Domain.Entities
{
    public class SystemConfig
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
