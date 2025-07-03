using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;

namespace MeoMeo.CMS.IServices
{
    public interface IMaterialClientService
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>> GetAllMaterialsAsync(GetListMaterialRequest request);
        Task<CreateOrUpdateMaterialDTO> GetMaterialsByIdAsync(Guid id);
        Task<CreateOrUpdateMaterialResponse> CreateMaterialsAsync(CreateOrUpdateMaterialDTO material);
        Task<CreateOrUpdateMaterialResponse> UpdateMaterialsAsync(CreateOrUpdateMaterialDTO material);
        Task<bool> DeleteMaterialsAsync(Guid id);
    }
}
