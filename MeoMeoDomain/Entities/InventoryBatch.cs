using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class InventoryBatch : BaseEnitityAudited
    {
        public float OriginalPrice { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
    }
}
