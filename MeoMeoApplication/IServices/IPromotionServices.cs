using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IPromotionServices
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO>> GetAllPromotionAsync(GetListPromotionRequestDTO request);
        Task<CreateOrUpdatePromotionResponseDTO> GetPromotionByIdAsync(Guid id);
        Task<Promotion> CreatePromotionAsync(CreateOrUpdatePromotionDTO promotion);
        Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionAsync(CreateOrUpdatePromotionDTO promotion);
        Task<bool> DeletePromotionAsync(Guid id);
    }
}
