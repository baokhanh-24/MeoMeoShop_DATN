using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Contract.DTOs.PromotionDetail;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.CMS.IServices
{
    public interface IPromotionDetailClientService
    {
        Task<PagedResult<CreateOrUpdatePromotionDetailDTO>> GetAllPromotionDetailAsync(GetListPromotionDetailRequestDTO request);
        Task<CreateOrUpdatePromotionDetailDTO> GetPromotionDetailByIdAsync(Guid id);
        Task<CreateOrUpdatePromotionDetailResponseDTO> CreateAsync(CreateOrUpdatePromotionDetailDTO dto);
        Task<CreateOrUpdatePromotionDetailResponseDTO> UpdateAsync(CreateOrUpdatePromotionDetailDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
