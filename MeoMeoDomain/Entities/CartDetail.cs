using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class CartDetail : BaseEntity
    {
        public float Discount {  get; set; }
        public int Quantity {  get; set; }
        public float Price {  get; set; }



        public Guid CartId {  get; set; }
        public Guid ProductDetailId {  get; set; }
        public Guid PromotionDetailId {  get; set; }
    }
}
