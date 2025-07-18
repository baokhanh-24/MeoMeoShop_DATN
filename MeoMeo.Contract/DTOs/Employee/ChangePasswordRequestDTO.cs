using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Employees
{
    public class ChangePasswordRequestDTO
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
