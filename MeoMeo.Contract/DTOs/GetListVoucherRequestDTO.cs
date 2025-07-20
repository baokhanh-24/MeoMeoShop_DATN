using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs
{
    public class GetListVoucherRequestDTO : BasePaging
    {
    
            public string? CodeFilter { get; set; }
            public string? NameFilter { get; set; }
            public EVoucherType? TypeFilter { get; set; }
            public DateTime? StartDateFromFilter { get; set; }
            public DateTime? StartDateToFilter { get; set; }
            public DateTime? EndDateFromFilter { get; set; }
            public DateTime? EndDateToFilter { get; set; }
            public EVoucherStatus? Status { get; set; } // dùng cho tab trạng thái
        

           // public bool? IsActive { get; set; }

       
    }
}
