using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.InventoryBatch
{
    public class GetListInventoryBatchRequestDTO : BasePaging
    {
        public float? OriginalPriceFilter { get; set; }
        public string? CodeFilter { get; set; }
        public int? QuantityFilter { get; set; }
        public string? NoteFilter { get; set; }
        public EInventoryBatchStatus? StatusFilter { get; set; }
    }
}
