using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public string Sku { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public float Discount { get; set; }
        public string Note { get; set; }
        public string Image { get; set; }


        public Guid OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid PromotionDetailId { get; set; }
        public Guid IventoryBatchId { get; set; }
    }
}
