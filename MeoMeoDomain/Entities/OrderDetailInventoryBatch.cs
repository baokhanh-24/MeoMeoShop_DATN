using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;

public class OrderDetailInventoryBatch:BaseEntity
{
    public Guid OrderDetailId { get; set; }
    public Guid InventoryBatchId { get; set; }
    public int Quantity { get; set; }
    public virtual OrderDetail OrderDetail { get; set; }
    public virtual InventoryBatch InventoryBatch { get; set; }
}