using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.SystemConfig
{
    public class SystemConfigDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ESystemConfigType Type { get; set; }
    }
}
