using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EVoucherStatus
    {
        All = 0,         // Hiển thị tất cả
        Upcoming = 1,    // Sắp diễn ra (StartDate > Now)
        Active = 2,      // Đang diễn ra (StartDate <= Now <= EndDate)
        Expired = 3      // Đã kết thúc (EndDate < Now)
    }
}
