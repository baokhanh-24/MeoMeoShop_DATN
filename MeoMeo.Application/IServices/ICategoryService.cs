using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryResponseDTO> UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryResponseDTO> DeleteCategoryAsync(Guid id);
    }
} 