using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class GetListBrandRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
        public string? CountryFilter { get; set; }
        public string? CodeFilter { get; set; }
        public int? EstablishYearFilter { get; set; }
    }
}
