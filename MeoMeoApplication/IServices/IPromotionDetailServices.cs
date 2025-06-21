using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IPromotionDetailServices
    {
        Task<List<PromotionDetail>> GetAllPromotionDetailAsync();
        Task<CreateOrUpdatePromotionDetailResponseDTO> GetPromotionDetailByIdAsync(Guid id);
        Task<PromotionDetail> CreatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail);
        Task<CreateOrUpdatePromotionDetailResponseDTO> UpdatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail);
        Task<bool> DeletePromotionDetailAsync(Guid id);
    }
}
