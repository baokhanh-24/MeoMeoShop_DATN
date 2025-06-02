using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class PromotionDetail
    {
        public Guid Id { get; set; }
        public float Discount { get; set; }
        public string Note { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }


        public Guid PromotionId { get; set; }
    }
}
