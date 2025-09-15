using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs.InventoryBatch
{
    public class GetListInventoryBatchRequestDTO : BasePaging
    {
        public float? OriginalPriceFilter { get; set; }
        public string? CodeFilter { get; set; }
        public int? QuantityFilter { get; set; }
        public string? NoteFilter { get; set; }
        public EInventoryBatchStatus? StatusFilter { get; set; }
        public Guid? ImportBatchIdFilter { get; set; }
        public Guid? ProductDetailIdFilter { get; set; }
    }
}
