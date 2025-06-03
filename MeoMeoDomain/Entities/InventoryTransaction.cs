namespace MeoMeo.Domain.Entities
{
    public class InventoryTransaction
    {
        public Guid Id { get; set; }
        public Guid InventoryBatchId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreateBy { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public virtual InventoryBatch InventoryBatch { get; set; }
    }
}
