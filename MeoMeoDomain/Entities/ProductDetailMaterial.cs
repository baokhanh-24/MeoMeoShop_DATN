using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class ProductDetailMaterial
    {
        public Guid ProductDetailId { get; set; }
        public Guid MaterialId { get; set; }
    }
}
