using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Size
{
    public class GetListSizeRequestDTO : BasePaging
    {
        public string? ValueFilter { get; set; }
        public string? CodeFilter { get; set; }
        public ESizeStatus? StatusFilter { get; set; }
    }
}
