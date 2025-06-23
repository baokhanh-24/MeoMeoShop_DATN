using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CartDetailResponseDTO : BaseResponse
    {
        public Guid? Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public Guid PonmotionId { get; set; }
        public float Discount { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}
