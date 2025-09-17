using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class GetListColourRequest : BasePaging
    {
        public string? NameFilter { get; set; }
        public string? CodeFilter { get; set; }
        public EColourStatus? StatusFilter { get; set; }
    }
}
