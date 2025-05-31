using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class Product : BaseEnitityAudited
    {
        public string Name { get; set; }
    }
}
