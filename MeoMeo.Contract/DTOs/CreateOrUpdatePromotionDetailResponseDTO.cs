using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdatePromotionDetailResponseDTO : BaseResponse
    {
        public Guid PromotionId { get; set; }
        public Guid Id { get; set; }
        public float Discount { get; set; }
        public string Note { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
    }
}
