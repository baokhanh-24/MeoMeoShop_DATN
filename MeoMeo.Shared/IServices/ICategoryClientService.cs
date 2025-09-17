using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Shared.IServices
{
    public interface ICategoryClientService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryResponseDTO> UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryResponseDTO> DeleteCategoryAsync(Guid id);
        Task<CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(Guid categoryId, int take = 6);
    }
} 