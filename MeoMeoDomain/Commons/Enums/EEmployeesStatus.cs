using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum EEmployeesStatus
    {
        [Display(Name = "Ngừng hoạt động")]
        Disabled,
        [Display(Name = "Đang hoạt động")]
        Enabled,
        [Display(Name = "Bị khóa")]
        Locked
    }
}
