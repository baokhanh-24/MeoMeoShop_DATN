using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class GetListMaterialRequest : BasePaging
    {
        public string? NameFilter { get; set; }
        public EMaterialDurability? DurabilityFilter { get; set; }
        public bool? WaterProofFilter { get; set; }
        public EMaterialWeight? WeightFilter { get; set; }
        public string? DescriptionFilter { get; set; }
        public EMaterialStatus? StatusFilter { get; set; }
    }
}
