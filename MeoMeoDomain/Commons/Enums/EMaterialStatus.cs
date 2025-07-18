using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EMaterialStatus
    {
        [Display(Name = "Không hoạt động")]
        Inactive = 0,

        [Display(Name = "Đang hoạt động")]
        Active = 1,

        [Display(Name = "Đã xóa")]
        Deleted = 2
    }
}
