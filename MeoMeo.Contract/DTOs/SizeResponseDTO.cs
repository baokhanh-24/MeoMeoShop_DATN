using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class SizeResponseDTO : BaseResponse
    {
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
    }
}
