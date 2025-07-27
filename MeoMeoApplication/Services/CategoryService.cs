using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            category.Id = Guid.NewGuid();
            
            await _categoryRepository.AddAsync(category);
            return new CategoryResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm thành công"
            };
        }

        public async Task<CategoryResponseDTO> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return new CategoryResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }    

            await _categoryRepository.DeleteAsync(id);
            return new CategoryResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Xóa thành công" };
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories;
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if(category == null)
            {
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy Category với ID: {id}"
                };
            }
            
            var response = _mapper.Map<CategoryResponseDTO>(category);
            response.ResponseStatus = BaseStatus.Success;
            response.Message = "Lấy dữ liệu thành công";
            return response;
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryDTO.Id.Value);
            if (category == null)
            {
                return new CategoryResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            
            _mapper.Map(categoryDTO, category);
            await _categoryRepository.UpdateAsync(category);
            
            return new CategoryResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
} 