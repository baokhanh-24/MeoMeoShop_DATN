using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;

public class OrderReturnItem : BaseEntity
{
    public Guid OrderReturnId { get; set; }
    public Guid OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }

    public virtual OrderReturn OrderReturn { get; set; }
    public virtual OrderDetail OrderDetail { get; set; }
}


