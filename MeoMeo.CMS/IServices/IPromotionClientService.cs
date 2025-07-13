using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Domain.Commons;

namespace MeoMeo.CMS.IServices
{
    public interface IPromotionClientService
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO>> GetAllPromotionAsync(GetListPromotionRequestDTO request);
        Task<CreateOrUpdatePromotionDTO> GetPromotionByIdAsync(Guid id);
        Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionAsync(CreateOrUpdatePromotionDTO dto);
        Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionAsync(CreateOrUpdatePromotionDTO dto);
        Task<bool> DeletePromotionAsync(Guid id);
    }
}
