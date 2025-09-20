using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class GetListCategoryRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
        public string? CodeFilter { get; set; }
        public string? DescriptionFilter { get; set; }
        public bool? StatusFilter { get; set; }
    }
} 