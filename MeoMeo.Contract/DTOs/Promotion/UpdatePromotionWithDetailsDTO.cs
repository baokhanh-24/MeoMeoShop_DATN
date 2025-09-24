using System;
using System.Collections.Generic;
using MeoMeo.Contract.DTOs.PromotionDetail;

namespace MeoMeo.Contract.DTOs.Promotion
{
    public class UpdatePromotionWithDetailsDTO
    {
        public Guid PromotionId { get; set; }
        public CreateOrUpdatePromotionDTO Promotion { get; set; } = new();
        public List<CreateOrUpdatePromotionDetailDTO> PromotionDetails { get; set; } = new();
    }
}
