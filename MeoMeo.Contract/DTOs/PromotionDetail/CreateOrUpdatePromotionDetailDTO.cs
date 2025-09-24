using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.PromotionDetail
{
    public class CreateOrUpdatePromotionDetailDTO
    {
        public Guid? PromotionId { get; set; }
        public Guid? ProductDetailId { get; set; }
        public Guid? Id { get; set; }
        public float Discount { get; set; }
        public string? Note { get; set; }

    }
}
