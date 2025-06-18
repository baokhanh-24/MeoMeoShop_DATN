using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class ProductDetaillSizeDTO
    {
        public Guid SizeId { get; set; }
        public Guid ProductDetaillId { get; set; }
    }
}
