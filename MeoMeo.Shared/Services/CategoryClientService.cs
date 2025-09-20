using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.Shared.Services
{
    public class CategoryClientService : ICategoryClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<CategoryClientService> _logger;

        public CategoryClientService(IApiCaller httpClient, ILogger<CategoryClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                var url = "/api/Category";
                var response = await _httpClient.GetAsync<IEnumerable<Category>>(url);
                return response ?? new List<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Category: {Message}", ex.Message);
                return new List<Category>();
            }
        }

        public async Task<PagedResult<CategoryDTO>> GetAllCategoriesPagedAsync(GetListCategoryRequestDTO request)
        {
            try
            {
                var url = "/api/Category/get-all-categories-paged";
                var response = await _httpClient.GetAsync<PagedResult<CategoryDTO>>(url);
                return response ?? new PagedResult<CategoryDTO>
                {
                    TotalRecords = 0,
                    PageIndex = 1,
                    PageSize = 10,
                    Items = new List<CategoryDTO>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Category phân trang: {Message}", ex.Message);
                return new PagedResult<CategoryDTO>
                {
                    TotalRecords = 0,
                    PageIndex = 1,
                    PageSize = 10,
                    Items = new List<CategoryDTO>()
                };
            }
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/Category/{id}";
                var response = await _httpClient.GetAsync<CategoryResponseDTO>(url);
                return response ?? new CategoryResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy Category theo Id {Id}: {Message}", id, ex.Message);
                return new CategoryResponseDTO();
            }
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var url = "/api/Category";
                var result = await _httpClient.PostAsync<CategoryDTO, CategoryResponseDTO>(url, categoryDTO);
                return result ?? new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo Category: {Message}", ex.Message);
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo Category"
                };
            }
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var url = $"/api/Category/{categoryDTO.Id}";
                var result = await _httpClient.PutAsync<CategoryDTO, CategoryResponseDTO>(url, categoryDTO);
                return result ?? new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật Category: {Message}", ex.Message);
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật Category"
                };
            }
        }

        public async Task<CategoryResponseDTO> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var url = $"/api/Category/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Xóa Category thất bại với Id {Id}", id);
                }
                return new CategoryResponseDTO
                {
                    ResponseStatus = result ? BaseStatus.Success : BaseStatus.Error,
                    Message = result ? "Xóa thành công" : "Xóa thất bại"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá Category {Id}: {Message}", id, ex.Message);
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi xóa Category"
                };
            }
        }

        public async Task<CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(Guid categoryId, int take = 6)
        {
            try
            {
                var url = $"/api/Category/hover-preview/{categoryId}?take={take}";
                var response = await _httpClient.GetAsync<CategoryHoverResponseDTO>(url);
                return response ?? new CategoryHoverResponseDTO
                {
                    CategoryId = categoryId,
                    CategoryName = string.Empty,
                    Products = new List<ProductPreviewDTO>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm theo Category {Id}: {Message}", categoryId, ex.Message);
                return new CategoryHoverResponseDTO
                {
                    CategoryId = categoryId,
                    CategoryName = string.Empty,
                    Products = new List<ProductPreviewDTO>()
                };
            }
        }
    }
}