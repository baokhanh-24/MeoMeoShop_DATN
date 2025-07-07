using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class GetlistEmployeesRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
        public string? CodeFilter { get; set; }
        public string? PhoneNumberFilter { get; set; }
        public DateTime? DateOfBirthFilter { get; set; }
        public string? AddressFilter { get; set; }
        public EEmployeesStatus? StatusFilter { get; set; }
    }
}
