using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class Image : BaseEntity
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string URL { get; set; }


        public Guid ProductDetailId { get; set; }
    }
}
