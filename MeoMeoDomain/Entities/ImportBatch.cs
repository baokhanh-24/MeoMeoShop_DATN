using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class ImportBatch : BaseEntityAudited
    {
        public string Code { get; set; }
        public DateTime ImportDate { get; set; }
        public string Note { get; set; }

        // Navigation properties
        public virtual ICollection<InventoryBatch> InventoryBatches { get; set; } = new List<InventoryBatch>();
    }
}
