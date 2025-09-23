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
        Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>> GetAllPromotionAsync(GetListPromotionRequestDTO request);
        Task<CreateOrUpdatePromotionResponseDTO> GetPromotionByIdAsync(Guid id);
        Task<GetPromotionDetailResponseDTO> GetPromotionDetailAsync(Guid id);
        Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionAsync(CreateOrUpdatePromotionDTO promotion);
        Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionWithDetailsAsync(UpdatePromotionWithDetailsDTO request);
        Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionAsync(CreateOrUpdatePromotionDTO promotion);
        Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionWithDetailsAsync(UpdatePromotionWithDetailsDTO request);
        Task<bool> DeletePromotionAsync(Guid id);
    }
}
