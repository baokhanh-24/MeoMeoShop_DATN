using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.SystemConfig
{
    public class GetListSystemConfigRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
        public string? ValueFilter { get; set; }
        public ESystemConfigType? TypeFilter { get; set; }

    }
}
