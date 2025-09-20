using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.Shared.IServices
{
    public interface ICategoryClientService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<PagedResult<CategoryDTO>> GetAllCategoriesPagedAsync(GetListCategoryRequestDTO request);
        Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryResponseDTO> UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryResponseDTO> DeleteCategoryAsync(Guid id);
        Task<CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(Guid categoryId, int take = 6);
    }
}