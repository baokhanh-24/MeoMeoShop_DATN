using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class ProductDetaillColourResponseDTO : BaseResponse
    {
        public Guid ColourId { get; set; }
        public Guid ProductDetaillId { get; set; }
    }
}
