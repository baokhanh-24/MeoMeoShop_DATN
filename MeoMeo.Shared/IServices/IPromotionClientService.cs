using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IPromotionClientService
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>> GetAllPromotionAsync(GetListPromotionRequestDTO request);
        Task<CreateOrUpdatePromotionResponseDTO> GetPromotionByIdAsync(Guid id);
        Task<GetPromotionDetailResponseDTO> GetPromotionDetailAsync(Guid id);
        Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionAsync(CreateOrUpdatePromotionDTO dto);
        Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionWithDetailsAsync(UpdatePromotionWithDetailsDTO request);
        Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionAsync(CreateOrUpdatePromotionDTO dto);
        Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionWithDetailsAsync(UpdatePromotionWithDetailsDTO request);
        Task<bool> DeletePromotionAsync(Guid id);
    }
}
