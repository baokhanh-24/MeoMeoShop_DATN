using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class InventoryBatch : BaseEntityAudited
    {
        public Guid ProductDetailId { get; set; }
        public float OriginalPrice { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public EInventoryBatchStatus Status { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<OrderDetailInventoryBatch> OrderDetailInventoryBatches { get; set; } = new List<OrderDetailInventoryBatch>();

    }
}
