using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;

public class OrderReturnFile : BaseEntity
{
    public Guid OrderReturnId { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }

    public virtual OrderReturn OrderReturn { get; set; }
}


