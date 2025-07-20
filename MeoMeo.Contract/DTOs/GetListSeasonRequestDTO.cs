using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class GetListSeasonRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
        public int? StatusFilter { get; set; }


    }
}
